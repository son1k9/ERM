CREATE TABLE user_event(
    user_id INTEGER NOT NULL,
    event_id INTEGER NOT NULL,
    FOREIGN KEY(user_id) REFERENCES user(id),
    FOREIGN KEY(event_id) REFERENCES event(id),
    PRIMARY KEY(user_id, event_id)
)