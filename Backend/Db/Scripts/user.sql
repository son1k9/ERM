CREATE TABLE user(
    id INTEGER PRIMARY KEY,
    email TEXT NOT NULL,
    login TEXT NOT NULL,
    phone TEXT NOT NULL,
    hashed_password TEXT NOT NULL
)