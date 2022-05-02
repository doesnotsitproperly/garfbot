# range(), but inclusive
def inclusive_range(min: int, max: int) -> range:
    return range(min, max + 1)

# len(), but exclusive
def index_len(var: str | list) -> int:
    return len(var) - 1

# Parse a string and only return the digits
def int_from_str(s: str) -> int:
    new_str = ""
    for char in s:
        if char.isdigit():
            new_str += char
    return int(new_str)
