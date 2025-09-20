import { sha256 } from 'js-sha256';

export const codeVerifier = () => {
  const array = new Uint32Array(56 / 2);
  window.crypto.getRandomValues(array);
  return Array.from(array, (elm) => '0' + elm.toString(16).substring(-2)).join('');
};

const base64UrlEncode = (arrayBuffer: ArrayBuffer): string => {
  let str = '';
  const bytes = new Uint8Array(arrayBuffer);
  const chunkSize = 0x8000;
  for (let i = 0; i < bytes.length; i += chunkSize) {
    str += String.fromCharCode(...Array.from(bytes.subarray(i, i + chunkSize)));
  }
  return btoa(str).replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
};
export const generateCodeChallange = (verifier: string) => {
  const hash = sha256.arrayBuffer(verifier);
  return base64UrlEncode(hash);
};

export const generateState = () => {
  return codeVerifier();
};
