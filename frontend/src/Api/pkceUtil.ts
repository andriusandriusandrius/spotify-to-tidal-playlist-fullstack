import base64url from 'base64url';
import { sha256 } from 'js-sha256';

export const codeVerifier = () => {
  const array = new Uint32Array(56 / 2);
  window.crypto.getRandomValues(array);
  return Array.from(array, (elm) => '0' + elm.toString(16).substring(-2)).join('');
};

export const generateCodeChallange = (verifier: string) => {
  const hash = sha256.arrayBuffer(verifier);
  return base64url(Buffer.from(hash));
};

export const generateState = () => {
  return codeVerifier();
};
