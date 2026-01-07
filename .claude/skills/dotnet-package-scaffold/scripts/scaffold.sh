#!/bin/bash

# .NET 8 Package Structure Scaffolding Script
# Automates creation of a .NET solution with reusable package and main app
# Usage: ./scaffold.sh <solution-name> <package-name> <app-name> <db-provider>

set -e

SOLUTION_NAME=${1:-"DemoApp"}
PACKAGE_NAME=${2:-"SharedPackage"}
APP_NAME=${3:-"MainApp"}
DB_PROVIDER=${4:-"sqlite"}

echo "╔════════════════════════════════════════════════════════════╗"
echo "║   .NET 8 Package Structure Scaffolder                      ║"
echo "╚════════════════════════════════════════════════════════════╝"
echo ""
echo "Solution:      $SOLUTION_NAME"
echo "Package:       $PACKAGE_NAME"
echo "Main App:      $APP_NAME"
echo "DB Provider:   $DB_PROVIDER"
echo ""

# Validate DB provider
case "$DB_PROVIDER" in
    sqlite|sqlserver|postgres)
        ;;
    *)
        echo "❌ Invalid database provider: $DB_PROVIDER"
        echo "Valid options: sqlite, sqlserver, postgres"
        exit 1
        ;;
esac

# Create root directory
if [ -d "$SOLUTION_NAME" ]; then
    echo "❌ Directory $SOLUTION_NAME already exists!"
    exit 1
fi

mkdir "$SOLUTION_NAME"
cd "$SOLUTION_NAME"

echo "📁 Creating directory structure..."

# Create solution
dotnet new sln -n "$SOLUTION_NAME" > /dev/null

# Create project directories
mkdir -p "src/$APP_NAME" "src/$PACKAGE_NAME"

echo "📦 Creating projects..."

# Create console app
dotnet new console -o "src/$APP_NAME" -n "$APP_NAME" > /dev/null

# Create class library
dotnet new classlib -o "src/$PACKAGE_NAME" -n "$PACKAGE_NAME" > /dev/null

# Add projects to solution
dotnet sln add "src/$APP_NAME/$APP_NAME.csproj" > /dev/null
dotnet sln add "src/$PACKAGE_NAME/$PACKAGE_NAME.csproj" > /dev/null

echo "📚 Adding NuGet packages..."

# Add EF Core packages to main app
cd "src/$APP_NAME"

case "$DB_PROVIDER" in
    sqlite)
        dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 8.0.* > /dev/null
        ;;
    sqlserver)
        dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.* > /dev/null
        ;;
    postgres)
        dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.* > /dev/null
        ;;
esac

dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.* > /dev/null

# Add reference to package
dotnet add reference "../$PACKAGE_NAME/$PACKAGE_NAME.csproj" > /dev/null

# Add EF Core to package
cd "../$PACKAGE_NAME"
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.* > /dev/null

# Remove default Class1.cs
rm -f Class1.cs

cd ../..

echo "✅ Solution scaffolded successfully!"
echo ""
echo "📂 Project structure:"
echo "   $SOLUTION_NAME/"
echo "   ├── $SOLUTION_NAME.sln"
echo "   └── src/"
echo "       ├── $APP_NAME/"
echo "       └── $PACKAGE_NAME/"
echo ""
echo "Next steps:"
echo "  1. cd $SOLUTION_NAME"
echo "  2. Add your models to src/$PACKAGE_NAME/Models/"
echo "  3. Add your models to src/$APP_NAME/Models/"
echo "  4. Create DbContext in src/$APP_NAME/Data/"
echo "  5. Update Program.cs with your logic"
echo "  6. Run: dotnet build"
echo "  7. Run: dotnet run --project src/$APP_NAME"
echo ""
echo "To package the library:"
echo "  cd src/$PACKAGE_NAME"
echo "  dotnet pack -c Release"
