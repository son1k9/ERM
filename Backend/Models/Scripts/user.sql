CREATE TABLE user(
    id INTEGER PRIMARY KEY,
    email TEXT UNIQUE NOT NULL,
    login TEXT UNIQUE NOT NULL,
    hashed_password TEXT NOT NULL
)