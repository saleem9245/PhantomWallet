#!/bin/bash
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
TERM=xterm $DIR/PhantomWallet --port=7071 --path=$DIR/www