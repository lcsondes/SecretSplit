# Secret Splitter
This is an implementation of [Shamir's Secret Sharing](https://en.wikipedia.org/wiki/Shamir%27s_Secret_Sharing) that I made because I found [existing ones](https://www.xkcd.com/927) too cumbersome to use. On my machine, this program compiles to a 15 kB executable that you can just double click and go.

## Usage
Enter your secret in the top row (it will be displayed in plaintext), configure the total number of fragments (right) and the required number of fragments (left) to recombine the input. Press "Split secret". Each line that appears is a part of the secret that you can hand out to trustees.

Going backwards, you should paste all fragments in your possession into the large text field (it can be more than required), and click "Merge secret". It is not necessary to change the numbers this time, the strings contain all the parameters.

## Disclaimer
**This is home-grown crypto**. That means that although SSS is cryptographically sound, it's also very likely that this implementation is not secure. You should not assume that the input can only be recovered in possession of the configured number of strings, or that even if you have those strings, the input can always be recovered.

## Configuration
Look at the top of Crypto.cs for the maximum number of bits allowed, and the public prime used for the finite field. Adjust them as necessary for your usage.

## License

Secret Splitter is licensed under the terms of the AGPLv3, or, at your choice, any later version as published by the Free Software Foundation.