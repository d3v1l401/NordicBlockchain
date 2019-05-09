#!/bin/bash

command -v openssl > /dev/null 2>&1 || { echo >&2 "[!] OpenSSL is not installed!"; exit 1; }

echo "[+] Generating private key..."
openssl genrsa -des3 -out miner_privKey.pem 2048

echo "[+] Exporting public key..."
openssl rsa -in miner_privKey.pem -outform PEM -pubout -out miner_pubKey.pem

echo "[+] Exporting unencrypted(!!) RSA private key..."
openssl rsa -in miner_privKey.pem -out miner_privKeyOut.pem -outform PEM
