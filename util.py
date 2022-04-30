import json

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

# json.loads(), but with support single-line comments
def jsonc_loads(s: str) -> dict:
    s_list = s.split('\n')
    for i in inclusive_range(0, index_len(s_list)):
        line = s_list[i]
        if "//" in line:
            new_line = line[0 : line.find("//")]
            s_list[i] = new_line
    return json.loads('\n'.join(s_list))
