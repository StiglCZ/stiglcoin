# StiglCoin

### This repository contains API for stiglcoin currency
[-] - System of creating custom wallet to send money from

### Creating wallet
`GET/cre&<name>`
    - output: <PUBLIC_KEY>\n<PRIVATE_KEY>
### Getting wallet info
`GET/<name>`
    - output: <PUBLIC_KEY>\n<BALANCE>
### Creating a paying wallet(aka. card)
`GET/cca&<account_PUBLIC_key>&<account_PRIVATE_key>&<credit>`
    - output: <PUBLIC_KEY>\n<PRIVATE_KEY>
### Paying with paying wallet(card)
`GET/pay&<source_PUBLIC_key>&<dest_PUBLIC_key>&<source_PRIVATE_key>&<value>`
### Veryfying valid login
`GET/ver&<name/id>&<key>`