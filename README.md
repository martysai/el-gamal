## El Gamal Encryption

To launch the project run .exe file.

## Purposes and Aims

The main objective of the project is to demonstrate possible ways of:
- Finite field (Galois field) implementation. The field maintains:
  - Prime order;
  - Non-Prime order - that is equal to _q=p<sup>n</sup>, p --_ prime number.
- The process of encoding, ciphering, decoding and deciphering the information - basic cryptosystem workflows.

## User's manual

A basic language of the program is Russian. User’s manual in Russian is detailed in the program. Let's consider main windows and how to work with them:
1. Main Window. It contains 3 buttons: Begin, Reference (in Russian), Info about project.
<img src="https://i.imgur.com/lBIht73.png" width="430">
  2. Mode Choice Window. It contains of 2 image buttons and 1 back button: the first provides the mode of the finite field over prime order, the second one provides non-prime order (implementation is represented by the field of polynomials):
<img src="https://downloader.disk.yandex.ru/preview/1ef01424c696742f0941584f4d45878a1f23f13bb5141ffdd2c3963a5a3f33e1/5c0b030d/wOaR_I4HoROg8O6u6xk91zrB-yVk-0FRWY88BLncmrEMez2XYo78RXVIfFWBk5OIDt4fcaMww4NfYhonk0znAQ%3D%3D?uid=0&filename=ModeWindow.png&disposition=inline&hash=&limit=0&content_type=image%2Fpng&tknv=v2&size=2048x2048" width="430">
  3. Cipher window provides a text field - a message that will be used in cryptosystem. Also you may choose text file. After pressing 1st button we will pass to the main cipher window.
<img src="https://downloader.disk.yandex.ru/preview/1c7fc51718d7a6c072c49a02b8e52b10bcf0a097a9af7282d033c8d4ea63b5b3/5c0b060d/wOaR_I4HoROg8O6u6xk919rmbnlI1OOkrbqIQePCKe33fq0OQCse1rGbvygPJJLr9HKmKa0MeIXepV6Y1ZgtBQ%3D%3D?uid=0&filename=CipherWindow.png&disposition=inline&hash=&limit=0&content_type=image%2Fpng&tknv=v2&size=2048x2048" width="430">
  4. This window will represent the key information about cryptosystem. "g" is a common polynomial. 
<img src="https://downloader.disk.yandex.ru/preview/25b37a3c2a7f3c79a1661c51a68d3c047d04d37256a070c503d3f9ed0397fec9/5c0b0645/wOaR_I4HoROg8O6u6xk91721ITFbr64oRsBSMzpe10xh_XNFFV_F8lSZutjI9yOqiTTNpp-SBP--DUTFdVKWwQ%3D%3D?uid=0&filename=AliceInfo.png&disposition=inline&hash=&limit=0&content_type=image%2Fpng&tknv=v2&size=2048x2048" width="430">
  5. After that you will see the information about encrypting/decrypting processes.
  
<img src="https://downloader.disk.yandex.ru/preview/6bd465ccce91c460ecc47b8d575a2f8b1bb8ac030458ff3f17513921305271fa/5c0b057e/wOaR_I4HoROg8O6u6xk9183ucwnRMycovqyCGnMOyXOnO8AdD_3k-FZPGXNKCslRXYcJPC04UhzMq_ZLlAT1oQ%3D%3D?uid=0&filename=CipherWindow2.png&disposition=inline&hash=&limit=0&content_type=image%2Fpng&tknv=v2&size=2048x2048" width="430">
<img src="https://downloader.disk.yandex.ru/preview/a5ea92c099e9fb3dfa59b798b285ff97f0013ca1cd647fab2d1e05bdfeabb220/5c0b069d/wOaR_I4HoROg8O6u6xk91x-p2wh6qBM16Nua05y9sSugrcG5PqRHKVq_5Xe1XL01vgZj6OcY6sasuwHlIUPIyA%3D%3D?uid=0&filename=DecipherWindow.png&disposition=inline&hash=&limit=0&content_type=image%2Fpng&tknv=v2&size=2048x2048" width="430">


## Theory

What do we need: to encrypt <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/0a07d98bb302f3856cbabc47b2b9016692e3f7bc" width="13"> to Alice under her public key <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/da0ac9490f7f974c22c1a32d48d8df1458fd7111" width="55">. It's noticeable that the length of input message might be large. That is why we will divide big message into parts of length 5 and will encrypt them independently.

- Encryption:
  - Bob chooses a random <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/b8a6208ec717213d4317e666f1ae872e00620a0d" width="7"> from <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/9c297585d481a11e0b0120f6fff03effee4e021c" width="77">, then calculates <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/680b1e7bee0bfb0ea74e56d6b1a220f97a27405a" width="60">.
  - Bob calculates the shared secret  <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/eb90c0b27a0b7d22dfd5e076fb47532915766059" width="105">.
  - Bob maps his message <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/0a07d98bb302f3856cbabc47b2b9016692e3f7bc" width="13"> onto an element <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/b2ea8347f7588b19652c2098395f059d76b12b60" width="20"> of <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/f5f3c8921a3b352de45446a6789b104458c9f90b" width="13">.
  - Bob calculates <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/56e180dff987c493025fa2b3fb9108d20a275017" width="80">.
  - Bob sends the ciphertext <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/c28d4f697204b4212fabd5b163f9274c5ab5d816" width="275"> to Alice.
  
- Decryption:
The decryption algorithm works as follows: to decrypt a ciphertext <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/40f16dff2d0a99e639b57fd5ebf52ef4c558f3a8" width="45"> with her private key <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/87f9e315fd7e2ba406057a97300593c4802b53e4" width="10">:
  - Alice calculates the shared secret <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/40f16dff2d0a99e639b57fd5ebf52ef4c558f3a8" width="45"> with her private key <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/87f9e315fd7e2ba406057a97300593c4802b53e4" width="10">.
  - and then computes <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/b554f79f314d732c6e0f7d3b370f3360f6f9f674" width="10"> which she then converts back into the plaintext message <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/0a07d98bb302f3856cbabc47b2b9016692e3f7bc" width="13">, where <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/68672877fe858638d659f82efca341f116d3fb8b" width="18"> is the inverse of <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/01d131dfd7673938b947072a13a9744fe997e632" width="7"> in the group <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/f5f3c8921a3b352de45446a6789b104458c9f90b" width="10">. (E.g. modular multiplicative inverse if <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/f5f3c8921a3b352de45446a6789b104458c9f90b" width="10"> is a subgroup of a multiplicative group of integers modulo n).
The decryption algorithm produces the intended message, since
    <img src="https://wikimedia.org/api/rest_v1/media/math/render/svg/cceee23463325d5656c228765a4d7f42dba0b226" width="430">

## Overall

This project mostly is educational, and therefore any improvements and suggestions would be preferable.
