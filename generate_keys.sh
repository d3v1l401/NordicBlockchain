#!/bin/bash

command -v openssl > /dev/null 2>&1 || { echo >&2 "[!] OpenSSL is not installed!"; exit 1; }

echo "[+] Generating private key..."
openssl genrsa -des3 -out privKey.pem 2048

echo "[+] Exporting public key..."
openssl rsa -in privKey.pem -outform PEM -pubout -out pubKey.pem

echo "[+] Exporting unencrypted(!!) RSA private key..."
openssl rsa -in privKey.pem -out privKeyOut.pem -outform PEM
