export const codeVerifier = () => {
  const array = new Uint32Array(56 / 2);
  window.crypto.getRandomValues(array);
  return Array.from(array, (elm) => '0' + elm.toString(16).substring(-2)).join('');
};
