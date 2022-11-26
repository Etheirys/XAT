from struct import pack

def write_string(file, value):
    bytes = value.encode('ascii')
    file.write(bytes)

def write_int(file, value):
    file.write(pack('<i', value))

def write_float(file, value):
    file.write(pack('<f', value))

def write_vector4(file, value):
    file.write(pack('<4f', *value))

def write_vector3(file, value):
    file.write(pack('<3f', *value))