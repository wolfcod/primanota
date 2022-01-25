#!/usr/bin/env python3
import argparse
import os
import sys
from os.path import basename
from os import path
from zipfile import ZipFile
import datetime

# parse_args
def parse_args(argv):
    argparser = argparse.ArgumentParser(epilog='primanota')
    argparser.add_argument('-i', '--input', help='Input File')
    argparser.add_argument('-o', '--output', help='Output File')
    #print("parse_args parsing =>", argv)
    args = argparser.parse_args(argv)
    print("Args parsed are: =>", args)
    args_dict = {
        'input': args.input,
        'output': args.output,
    }

    # Verifica gli input forniti...
    if args.input is None:
        print('Errore: input richiesto.')
        sys.exit(1)
    
    if args.output is None:
        print('Errore: output richiesto.')
        sys.exit(1)
     
    return args_dict

def primanota(args):
    print('Not yet implemented.')
    sys.exit(1)

def main(argv=sys.argv[1:]):
    args = parse_args(argv)
    if args is None:
        return 1

    seqNo = primanota(args)
    print(seqNo)

if __name__ == "__main__":
    main()