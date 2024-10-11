CREATE TABLE auth_token (
    hash BLOB PRIMARY KEY,
    expiry DATETIME NOT NULL,
    user_id INTEGER NOT NULL, 
    FOREIGN KEY(user_id) REFERENCES user(id)
)