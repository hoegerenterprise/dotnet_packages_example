# Flask Backend Structure

This guide covers setting up a Flask backend API to support a Vue 3 frontend application.

## Backend Purpose

The Flask backend provides:
- RESTful API endpoints for data operations
- JSON responses for frontend consumption
- CORS support for cross-origin requests
- Data persistence (database or file-based)
- Business logic and validation

## Project Structure

```
backend/
├── app.py                    # Main Flask application
├── models.py                 # Data models (SQLAlchemy or dataclasses)
├── routes/
│   ├── __init__.py
│   ├── items.py             # Item-related routes
│   └── orders.py            # Order-related routes
├── services/
│   ├── __init__.py
│   ├── item_service.py      # Business logic for items
│   └── order_service.py     # Business logic for orders
├── utils/
│   ├── __init__.py
│   └── helpers.py           # Utility functions
├── config.py                # Configuration settings
├── requirements.txt         # Python dependencies
├── .env                     # Environment variables (not committed)
├── .gitignore              # Git ignore file
└── README.md               # Backend documentation
```

## Basic Flask Application

### app.py

```python
from flask import Flask, jsonify, request
from flask_cors import CORS
import os

app = Flask(__name__)

# CORS configuration - allow frontend to access API
CORS(app, origins=[
    'http://localhost:5173',  # Main app dev server
    'http://localhost:5174',  # Playground dev server
    'http://localhost:5175',
    'http://localhost:5176',
    'http://localhost:5177',
])

# Configuration
app.config['JSON_SORT_KEYS'] = False
app.config['DEBUG'] = os.getenv('FLASK_DEBUG', 'False') == 'True'

# In-memory storage (replace with database in production)
items = []
next_id = 1


# ===== ROUTES =====

@app.route('/api/v1/health', methods=['GET'])
def health_check():
    """Health check endpoint"""
    return jsonify({
        'status': 'healthy',
        'version': '1.0.0'
    })


@app.route('/api/v1/items', methods=['GET'])
def get_items():
    """Get all items"""
    return jsonify(items)


@app.route('/api/v1/items/<item_id>', methods=['GET'])
def get_item(item_id):
    """Get single item by ID"""
    item = next((i for i in items if i['id'] == item_id), None)
    if not item:
        return jsonify({'error': 'Item not found'}), 404
    return jsonify(item)


@app.route('/api/v1/items', methods=['POST'])
def create_item():
    """Create new item"""
    global next_id
    data = request.json

    # Validation
    if not data or 'name' not in data:
        return jsonify({'error': 'Name is required'}), 400

    item = {
        'id': str(next_id),
        'name': data['name'],
        'description': data.get('description', ''),
        'status': data.get('status', 'pending'),
        'quantity': data.get('quantity', 0),
        'created_at': datetime.now().isoformat(),
    }
    next_id += 1

    items.append(item)
    return jsonify(item), 201


@app.route('/api/v1/items/<item_id>', methods=['PUT'])
def update_item(item_id):
    """Update existing item"""
    item = next((i for i in items if i['id'] == item_id), None)
    if not item:
        return jsonify({'error': 'Item not found'}), 404

    data = request.json
    item.update({
        'name': data.get('name', item['name']),
        'description': data.get('description', item['description']),
        'status': data.get('status', item['status']),
        'quantity': data.get('quantity', item['quantity']),
        'updated_at': datetime.now().isoformat(),
    })

    return jsonify(item)


@app.route('/api/v1/items/<item_id>', methods=['DELETE'])
def delete_item(item_id):
    """Delete item"""
    global items
    item = next((i for i in items if i['id'] == item_id), None)
    if not item:
        return jsonify({'error': 'Item not found'}), 404

    items = [i for i in items if i['id'] != item_id]
    return '', 204


# ===== ERROR HANDLERS =====

@app.errorhandler(400)
def bad_request(error):
    return jsonify({'error': 'Bad request'}), 400


@app.errorhandler(404)
def not_found(error):
    return jsonify({'error': 'Resource not found'}), 404


@app.errorhandler(500)
def internal_error(error):
    return jsonify({'error': 'Internal server error'}), 500


# ===== MAIN =====

if __name__ == '__main__':
    app.run(debug=True, port=5000, host='0.0.0.0')
```

## Data Models

### models.py (with dataclasses)

```python
from dataclasses import dataclass, field, asdict
from typing import List, Optional
from datetime import datetime
from enum import Enum


class ItemStatus(str, Enum):
    PENDING = 'pending'
    ACTIVE = 'active'
    COMPLETED = 'completed'


@dataclass
class Item:
    id: str
    name: str
    description: str = ''
    status: ItemStatus = ItemStatus.PENDING
    quantity: int = 0
    created_at: str = field(default_factory=lambda: datetime.now().isoformat())
    updated_at: Optional[str] = None

    def to_dict(self):
        """Convert to dictionary for JSON serialization"""
        data = asdict(self)
        data['status'] = self.status.value  # Convert enum to string
        return data

    @classmethod
    def from_dict(cls, data: dict):
        """Create instance from dictionary"""
        if 'status' in data:
            data['status'] = ItemStatus(data['status'])
        return cls(**data)


@dataclass
class Order:
    id: str
    internal_number: str
    status: str
    items: List[dict] = field(default_factory=list)
    created_at: str = field(default_factory=lambda: datetime.now().isoformat())
    updated_at: Optional[str] = None

    def to_dict(self):
        return asdict(self)

    @classmethod
    def from_dict(cls, data: dict):
        return cls(**data)
```

### models.py (with SQLAlchemy)

```python
from flask_sqlalchemy import SQLAlchemy
from datetime import datetime

db = SQLAlchemy()


class Item(db.Model):
    __tablename__ = 'items'

    id = db.Column(db.String(36), primary_key=True)
    name = db.Column(db.String(255), nullable=False)
    description = db.Column(db.Text)
    status = db.Column(db.String(50), default='pending')
    quantity = db.Column(db.Integer, default=0)
    created_at = db.Column(db.DateTime, default=datetime.utcnow)
    updated_at = db.Column(db.DateTime, onupdate=datetime.utcnow)

    def to_dict(self):
        return {
            'id': self.id,
            'name': self.name,
            'description': self.description,
            'status': self.status,
            'quantity': self.quantity,
            'created_at': self.created_at.isoformat() if self.created_at else None,
            'updated_at': self.updated_at.isoformat() if self.updated_at else None,
        }

    @classmethod
    def from_dict(cls, data: dict):
        return cls(
            id=data.get('id'),
            name=data['name'],
            description=data.get('description', ''),
            status=data.get('status', 'pending'),
            quantity=data.get('quantity', 0),
        )


class Order(db.Model):
    __tablename__ = 'orders'

    id = db.Column(db.String(36), primary_key=True)
    internal_number = db.Column(db.String(50), unique=True, nullable=False)
    status = db.Column(db.String(50), default='pending')
    created_at = db.Column(db.DateTime, default=datetime.utcnow)
    updated_at = db.Column(db.DateTime, onupdate=datetime.utcnow)

    # Relationships
    items = db.relationship('OrderItem', back_populates='order', cascade='all, delete-orphan')

    def to_dict(self):
        return {
            'id': self.id,
            'internal_number': self.internal_number,
            'status': self.status,
            'items': [item.to_dict() for item in self.items],
            'created_at': self.created_at.isoformat() if self.created_at else None,
            'updated_at': self.updated_at.isoformat() if self.updated_at else None,
        }
```

## Service Layer

### services/item_service.py

```python
from typing import List, Optional
from models import Item, ItemStatus
from datetime import datetime


class ItemService:
    def __init__(self):
        self.items: List[Item] = []
        self.next_id = 1

    def get_all(self) -> List[dict]:
        """Get all items"""
        return [item.to_dict() for item in self.items]

    def get_by_id(self, item_id: str) -> Optional[dict]:
        """Get item by ID"""
        item = next((i for i in self.items if i.id == item_id), None)
        return item.to_dict() if item else None

    def create(self, data: dict) -> dict:
        """Create new item"""
        item = Item(
            id=str(self.next_id),
            name=data['name'],
            description=data.get('description', ''),
            status=ItemStatus(data.get('status', 'pending')),
            quantity=data.get('quantity', 0),
        )
        self.next_id += 1
        self.items.append(item)
        return item.to_dict()

    def update(self, item_id: str, data: dict) -> Optional[dict]:
        """Update existing item"""
        item = next((i for i in self.items if i.id == item_id), None)
        if not item:
            return None

        item.name = data.get('name', item.name)
        item.description = data.get('description', item.description)
        item.status = ItemStatus(data.get('status', item.status.value))
        item.quantity = data.get('quantity', item.quantity)
        item.updated_at = datetime.now().isoformat()

        return item.to_dict()

    def delete(self, item_id: str) -> bool:
        """Delete item"""
        item = next((i for i in self.items if i.id == item_id), None)
        if not item:
            return False

        self.items = [i for i in self.items if i.id != item_id]
        return True

    def filter_by_status(self, status: str) -> List[dict]:
        """Get items by status"""
        filtered = [i for i in self.items if i.status.value == status]
        return [item.to_dict() for item in filtered]


# Singleton instance
item_service = ItemService()
```

## Blueprints for Route Organization

### routes/items.py

```python
from flask import Blueprint, jsonify, request
from services.item_service import item_service

bp = Blueprint('items', __name__, url_prefix='/api/v1/items')


@bp.route('', methods=['GET'])
def get_items():
    """Get all items, optionally filtered by status"""
    status = request.args.get('status')
    if status:
        items = item_service.filter_by_status(status)
    else:
        items = item_service.get_all()
    return jsonify(items)


@bp.route('/<item_id>', methods=['GET'])
def get_item(item_id):
    """Get single item"""
    item = item_service.get_by_id(item_id)
    if not item:
        return jsonify({'error': 'Item not found'}), 404
    return jsonify(item)


@bp.route('', methods=['POST'])
def create_item():
    """Create new item"""
    data = request.json

    # Validation
    if not data or 'name' not in data:
        return jsonify({'error': 'Name is required'}), 400

    try:
        item = item_service.create(data)
        return jsonify(item), 201
    except Exception as e:
        return jsonify({'error': str(e)}), 400


@bp.route('/<item_id>', methods=['PUT'])
def update_item(item_id):
    """Update item"""
    data = request.json
    item = item_service.update(item_id, data)
    if not item:
        return jsonify({'error': 'Item not found'}), 404
    return jsonify(item)


@bp.route('/<item_id>', methods=['DELETE'])
def delete_item(item_id):
    """Delete item"""
    success = item_service.delete(item_id)
    if not success:
        return jsonify({'error': 'Item not found'}), 404
    return '', 204
```

### app.py (with blueprints)

```python
from flask import Flask
from flask_cors import CORS
from routes import items

app = Flask(__name__)
CORS(app, origins=['http://localhost:5173', 'http://localhost:5174'])

# Register blueprints
app.register_blueprint(items.bp)

if __name__ == '__main__':
    app.run(debug=True, port=5000)
```

## Configuration

### config.py

```python
import os
from dotenv import load_dotenv

load_dotenv()


class Config:
    SECRET_KEY = os.getenv('SECRET_KEY', 'dev-secret-key')
    DEBUG = os.getenv('FLASK_DEBUG', 'False') == 'True'
    TESTING = False

    # Database
    SQLALCHEMY_DATABASE_URI = os.getenv(
        'DATABASE_URL',
        'sqlite:///app.db'
    )
    SQLALCHEMY_TRACK_MODIFICATIONS = False

    # CORS
    CORS_ORIGINS = [
        'http://localhost:5173',
        'http://localhost:5174',
        'http://localhost:5175',
    ]


class DevelopmentConfig(Config):
    DEBUG = True


class ProductionConfig(Config):
    DEBUG = False


config = {
    'development': DevelopmentConfig,
    'production': ProductionConfig,
    'default': DevelopmentConfig
}
```

## Dependencies

### requirements.txt

```txt
Flask==3.1.0
Flask-CORS==5.0.0
python-dotenv==1.0.1

# Optional: Database
Flask-SQLAlchemy==3.1.1
SQLAlchemy==2.0.36

# Optional: Validation
marshmallow==3.23.2
flask-marshmallow==1.2.1

# Optional: Authentication
Flask-JWT-Extended==4.7.1

# Optional: API Documentation
flask-swagger-ui==4.11.1
```

### Installation

```bash
cd backend
python -m venv venv
source venv/bin/activate  # On Windows: venv\Scripts\activate
pip install -r requirements.txt
```

## Environment Variables

### .env

```env
FLASK_DEBUG=True
SECRET_KEY=your-secret-key-here
DATABASE_URL=sqlite:///app.db

# API Configuration
API_VERSION=v1
API_PREFIX=/api

# CORS
CORS_ORIGINS=http://localhost:5173,http://localhost:5174
```

## .gitignore

### backend/.gitignore

```gitignore
# Python
__pycache__/
*.py[cod]
*$py.class
*.so
.Python

# Virtual Environment
venv/
ENV/
env/
.venv

# Flask
instance/
.webassets-cache

# Database
*.db
*.sqlite3

# Environment
.env
.flaskenv

# IDE
.vscode/
.idea/
*.swp
*.swo

# Testing
.pytest_cache/
.coverage
htmlcov/
*.cover

# Logs
*.log
```

## Running the Backend

### Development Mode

```bash
cd backend
source venv/bin/activate  # On Windows: venv\Scripts\activate
python app.py
```

Backend runs on `http://localhost:5000`

### With Flask CLI

```bash
export FLASK_APP=app.py
export FLASK_DEBUG=1
flask run
```

## API Testing

### Using curl

```bash
# Health check
curl http://localhost:5000/api/v1/health

# Get all items
curl http://localhost:5000/api/v1/items

# Get single item
curl http://localhost:5000/api/v1/items/1

# Create item
curl -X POST http://localhost:5000/api/v1/items \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Item","status":"pending"}'

# Update item
curl -X PUT http://localhost:5000/api/v1/items/1 \
  -H "Content-Type: application/json" \
  -d '{"status":"completed"}'

# Delete item
curl -X DELETE http://localhost:5000/api/v1/items/1
```

## Best Practices

1. **Use Blueprints** - Organize routes into logical groups
2. **Service Layer** - Separate business logic from routes
3. **Error Handling** - Use error handlers and return proper HTTP codes
4. **Validation** - Validate input data before processing
5. **CORS Configuration** - Only allow necessary origins
6. **Environment Variables** - Never commit secrets
7. **Type Hints** - Use Python type hints for better code quality
8. **Logging** - Use Flask's logger for debugging
9. **API Versioning** - Use `/api/v1/` prefix for version control
10. **Documentation** - Document API endpoints

## Common Response Patterns

### Success Response
```python
return jsonify({
    'data': result,
    'message': 'Operation successful'
}), 200
```

### Error Response
```python
return jsonify({
    'error': 'Error message',
    'details': additional_info
}), 400
```

### List Response
```python
return jsonify({
    'data': items,
    'total': len(items),
    'page': 1,
    'per_page': 10
}), 200
```

## Integration with Frontend

### Frontend Fetch Example

```typescript
// In Pinia store
const fetchItems = async () => {
  try {
    const response = await fetch('/api/v1/items')
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }
    items.value = await response.json()
  } catch (e) {
    console.error('Failed to fetch items:', e)
  }
}
```

### Vite Proxy Configuration

Frontend vite.config.ts must proxy API calls:

```typescript
server: {
  proxy: {
    '/api': {
      target: 'http://localhost:5000',
      changeOrigin: true,
    }
  }
}
```

This allows frontend to call `/api/v1/items` which gets proxied to `http://localhost:5000/api/v1/items`.
