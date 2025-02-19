# User

- GET /api/user - получение всех пользователей
- GET /api/user/{id} - получение пользователя по ID
- POST /api/user - создание нового пользователя
- PUT /api/user/{id} - обновление пользователя
- DELETE /api/user/{id} - удаление пользователя

# MenuItem

- GET /api/menuitem - получение всех позиций меню
- GET /api/menuitem/{id} - получение позиции меню по ID
- POST /api/menuitem - добавление позиции в меню
- PUT /api/menuitem/{id} - обновление позиции меню
- DELETE /api/menuitem/{id} - удаление позиции меню

# Cart

- GET /api/cart/{id} - получение корзины по ID
- GET /api/cart/user/{userId} - получение корзины пользователя
- POST /api/cart/user/{userId}/add-item - добавление товара в корзину
- DELETE /api/cart/user/{userId}/remove-item/{itemId} - удаление товара из корзины
- DELETE /api/cart/user/{userId}/clear - очистка корзины

# Order

- GET /api/order - получение всех заказов
- GET /api/order/{id} - получение заказа по ID
- GET /api/order/user/{userId} - получение заказов пользователя
- POST /api/order - создание нового заказа
- PUT /api/order/{id}/status - обновление статуса заказа
- DELETE /api/order/{id} - удаление заказа
