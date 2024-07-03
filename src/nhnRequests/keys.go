package nhnrequests

const (
	HttpKey    string = "a865d7e5e2458f8ce1b5ecd087e54594"
	DigestSalt string = "0bk2kvtFE2"
)

/*

in this comment, I'll explain how the entire encryption/compression/digestion pipeline works.
Before we add any digests or anything, if you are creating a response, it needs to be compressed using gzip. Requests are not compressed.
These requests/responses start with a 20 byte hash, which is calculated like this:
SHA1(salt + SHA1(salt + " " + content))

the salt is a constant.
After putting the hash before the contents, the entire result is encrypted using AES-128 ECB with the HttpKey and encoded using Base64URL.

*/
