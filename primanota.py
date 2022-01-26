#!/usr/bin/env python3
import argparse
import os
import sys
import csv
import json

# parse_args: input argv, output dictionary of argv
def parse_args(argv):
    argparser = argparse.ArgumentParser(epilog='primanota')
    argparser.add_argument('-i', '--input', help='Input File')
    argparser.add_argument('-o', '--output', help='Output File')
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

def match_line(testata, righe, rules, content):
    
    lines = 0

    for line in content:
        for r in rules:
            process = False

            if r['start'] is not None:
                start = r['start']
                field = line[r['field']]

                if field.startswith(start):
                    process = True
            
            if process:
                for o_r in r['righe']:
                    lines = lines + 1
                    print(o_r)

                
                print(line[r['field']])
    
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

    linea_testata = desc['testate']['fields'].split(';')
    linea_righe = desc['righe']['fields'].split(';')

    # definisce i set iniziali...
    testata = dict_csv(linea_testata, desc['testate']['values'])
    righe = dict_csv(linea_righe, desc['righe']['values'])
     
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