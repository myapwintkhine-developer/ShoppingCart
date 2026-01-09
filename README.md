# ShoppingCart

Add-Migration Initial_Create
Update-Database

----------------
Data Seed
User 
Email: user@gmail.com, Password: User123

Product
Code:P001, Name:Product A, Price: 1000.00,Balance:100.00
Code:P002, Name:Product B, Price: 2000.00,Balance:100.00
Code:P003, Name:Product C, Price: 3000.00,Balance:100.00

----------------
API Guide
BASE URL
https://localhost:7032

AUTHENTICATION
All endpoints require JWT token
Header: Authorization: Bearer {JWT_TOKEN}

LOGIN
POST /api/auth/login
Request Body:
{
  "email": "user@gmail.com",
  "password": "User123"
}

GET CURRENT CART (AUTO CREATE IF THERE IS NO ACTIVE CART YET)
GET /api/cart
Request Body: None

ADD ITEM TO CART
POST /api/cart/items
Request Body:
{
  "productId": "product-id",
  "quantity": 2
}

UPDATE ITEM QUANTITY
PUT /api/cart/items/{itemId}
Request Body:
5

REMOVE ITEM FROM CART
DELETE /api/cart/items/{itemId}
Request Body: None

CHECKOUT CART
POST /api/cart/checkout
Request Body: None

GET ALL USER'S CARTS (BOTH CURRENT AND ALREADY CHECKED-OUT CARTS)
GET /api/cart/all
Request Body: None

NOTES
One active cart per user. 
Cart is auto-created when adding items if there is no active cart. 
Empty active cart (with no items yet) is allowed. 
Finally Checkout the cart. 
JWT token is required for all cart endpoints.
Data Seeding for Product and User to simplify the app.

To Add in future
User Authorization such as Role-Based
Product CRUD
More Validation

