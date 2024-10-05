CREATE TABLE event(
    id INTEGER PRIMARY KEY,
    name TEXT NOT NULL,
    description TEXT NOT NULL,
    date DATETIME NOT NULL,
    place TEXT NOT NULL,
    canceled INTEGER NOT NULL DEFAULT 0,
    organizer_id INTEGER NOT NULL,
    FOREIGN KEY(organizer_id) REFERENCES user(id)
)