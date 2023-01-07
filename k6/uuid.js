import { uuidv4 } from 'https://jslib.k6.io/k6-utils/1.4.0/index.js';

export default function () {
    const randomUUID = uuidv4();
    return randomUUID;
}
