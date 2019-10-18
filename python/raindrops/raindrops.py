def convert(number):
    if not isinstance(number, int):
        raise Exception("Please enter a valid input")

    out = ''

    if (number % 3) == 0:
        out += 'Pling'

    if (number % 5) == 0:
        out += 'Plang'

    if (number % 7) == 0:
        out += 'Plong'

    if (out != ''):
        return out

    return str(number)