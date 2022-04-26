import random

# Parse a string and only return the digits
def get_int(s: str) -> int:
    new_str = ""
    for char in s:
        if char.isdigit():
            new_str += char
    return int(new_str)

# range(), but inclusive
def get_range(min: int, max: int) -> range:
    return range(min, max + 1)

# randrange(), but inclusive
def random_number(min: int, max: int) -> int:
    random.seed()
    return random.randrange(min, max + 1)
