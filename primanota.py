#!/usr/bin/env python3
import argparse
import os
import sys
import csv
import json

pnt_f = None
pnr_f = None

# parse_args: input argv, output dictionary of argv
def parse_args(argv):
    argparser = argparse.ArgumentParser(epilog='primanota')
    argparser.add_argument('-i', '--input', help='Input File')
    argparser.add_argument('-o', '--output', help='Output Suffix')
    argparser.add_argument('-j', '--json', help='JSON schema rules')

    #print("parse_args parsing =>", argv)
    args = argparser.parse_args(argv)

    args_dict = {
        'input': args.input,
        'output': args.output,
        'json': args.json,
    }

    fail = False

    if args.json is None:
        print('Error: JSON mancante.')
        fail = True

    # Verifica gli input forniti...
    if args.input is None:
        print('Errore: input richiesto.')
        fail = True

    if fail:
        sys.exit(1)

    return args_dict

# load_csv: Read an input file, and return a list of dictonary with each line
def load_csv(csv_name):
    content = []

    with open(csv_name, mode='r') as input:
        reader = csv.reader(input)
        print(reader)

        columns = next(reader)
        print(columns)

        lines = 1
        for row in reader:
            lines = lines + 1
            line = {}
            i = 0
            if len(row) == len(columns):
                for c in columns:
                    line[c]=row[i]
                    i = i + 1

                content.append(line)
            else:
                print('Riga %d saltata %d %d' %(lines, len(row), len(columns)) )

    return content

def load_rules(json_name):
    f = open(json_name)
    data = json.load(f)

    if data is None:
        print('Errore: Regole non caricate')
        sys.exit(2)

    if len(data['rules']) == 0:
        print('Nessuna regola definita nel file')
        sys.exit(3)

    print('Regole azienda %s caricate' %(data['codice_azienda']))

    f.close()
    return data

def match_beginwith(string, value):
    if string.startswith(value):
        return True
    return False

def match_contains(string, value):
    if value in string:
        return True
    return False

def match_equals(string, value):
    if string == value:
        return True
    return False

def match_not(string, value):
    if match_equals(string, value) and match_contains(string, value) and match_beginwith(string, value):
        return False
    return True

def write_header(f, r):
    print('Generazione %s' %(f))

    with open(f, 'a+', newline='') as csvfile:
        writer = csv.writer(csvfile, delimiter=';')
        writer.writerow(r)

def write_row(f, r):
    with open(f, 'a+', newline='') as csvfile:
        writer = csv.writer(csvfile, delimiter=';')
        writer.writerow(r)

# verify if r match something in line
def match_rule(r, line):
    if r is None:
        return False
    if line is None:
        return False

    content_to_evaluate = line[r['field']]    # content to evaluate
    criteria = r['rules']
    match_value = r['value']

    if content_to_evaluate is None:
        return False

    if len(content_to_evaluate) == 0:
        return False

    options = {
        "beginwith" : match_beginwith,
        "contains" : match_contains,
        "equals" : match_equals,
        "not" : match_not
    }

    return options[criteria](content_to_evaluate, match_value)

def match_line(testata, righe, rules, content):
    lines = 0

    for line in content:
        for r in rules:
            process = False

            match = match_rule(r, line)

            if match:
                riga_testata = prepara(testata, r['testata'], line)
                write_row(pnt_f, testata, 
                lines = lines + 1

    return lines

def dict_csv(fields, values):
    d = {}

    for f in fields:
        default_value = ""
        for v in values:
            if v['field'] == f:
                default_value = v['value']

        d[f] = default_value

    return d

def primanota(args):
    #csv.reader(args['input'])
    content = load_csv(args['input'])
    desc = load_rules(args['json'])

    pnt_f = 'pnt_' + args['output']
    pnr_f = 'pnr_' + args['output']

    header_testate = desc['testate']['fields'].split(';')
    header_righe = desc['righe']['fields'].split(';')

    write_header(pnt_f, header_testate)
    write_header(pnr_f, header_righe)

    # definisce i set iniziali...
    testata = dict_csv(header_testate, desc['testate']['values'])
    righe = dict_csv(header_righe, desc['righe']['values'])

    processed = match_line(testata, righe, desc['rules'], content)
    return processed

def main(argv=sys.argv[1:]):
    args = parse_args(argv)
    if args is None:
        return 1

    processed = primanota(args)
    print('Righe processate %d' %(processed))

if __name__ == "__main__":
    main()