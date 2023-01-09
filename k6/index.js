import uuid from './uuid.js';
import { sleep } from 'k6';
import http from 'k6/http';

export const options = {
    discardResponseBodies: true,
    scenarios: {
        contacts: {
            executor: 'ramping-vus',
            startVUs: 0,
            stages: [
                { duration: '30s', target: 100 },
                { duration: '20s', target: 100 },
                { duration: '10s', target: 0 },
            ],
            gracefulRampDown: '0s',
        },
    },
};

export default function () {
    const cartId = uuid();
    // Create cart through POST request
    const createRequest = {
        cartId: cartId
    }
    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };
    http.post('http://localhost:5095/cart', JSON.stringify(createRequest), params);
    // Get cart through GET request
    //    sleep(1);
    http.get('http://localhost:5095/cart/' + cartId);
    const productId = uuid();
    const addItemRequest = {
        productId: productId
    };
    const response = http.post(`http://localhost:5095/cart/${cartId}/items`, JSON.stringify(addItemRequest), params);
    //    sleep(1);
    http.get('http://localhost:5095/cart/' + cartId);
    //    sleep(1);
    http.post(`http://localhost:5095/cart/${cartId}/items/${productId}/increasequantity`, null, params);
    //    sleep(1);
    http.get('http://localhost:5095/cart/' + cartId);
    //    sleep(1);
    http.post(`http://localhost:5095/cart/${cartId}/items/${productId}/increasequantity`, null, params);
    //    sleep(1);
    http.get('http://localhost:5095/cart/' + cartId);
    //    sleep(1);
    http.post(`http://localhost:5095/cart/${cartId}/items/${productId}/decreasequantity`, null, params);
    //    sleep(1);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);

    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);

    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);
    http.get('http://localhost:5095/cart/' + cartId);

    //    sleep(1);
    http.del(`http://localhost:5095/cart/${cartId}/items/${productId}`, null, params);
    //    sleep(1);
    http.get('http://localhost:5095/cart/' + cartId);
}