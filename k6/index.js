import uuid from './uuid.js';
import { sleep } from 'k6';
import http from 'k6/http';

const host = "http://localhost:5000"

export const options = {
    thresholds: {
        http_req_failed: ['rate<0.01'], // http errors should be less than 1%
        http_req_duration: ['p(95)<200'], // 95% of requests should be below 200ms
    },
    discardResponseBodies: true,
    scenarios: {
        contacts: {
            executor: 'ramping-vus',
            startVUs: 0,
            stages: [
                { duration: '20s', target: 100 },
                { duration: '30s', target: 100 },
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
    http.post(`${host}/cart`, JSON.stringify(createRequest), params);
    // Get cart through GET request
    sleep(1);
    http.get(`${host}/cart/${cartId}`);
    const productId = uuid();
    const addItemRequest = {
        productId: productId
    };
    const response = http.post(`${host}/cart/${cartId}/items`, JSON.stringify(addItemRequest), params);
    sleep(1);
    http.get(`${host}/cart/${cartId}`);
    sleep(1);
    http.post(`${host}/cart/${cartId}/items/${productId}/increasequantity`, null, params);
    sleep(1);
    http.get(`${host}/cart/${cartId}`);
    sleep(1);
    http.post(`${host}/cart/${cartId}/items/${productId}/increasequantity`, null, params);
    sleep(1);
    http.get(`${host}/cart/${cartId}`);
    sleep(1);
    http.post(`${host}/cart/${cartId}/items/${productId}/decreasequantity`, null, params);
    sleep(1);
    http.get(`${host}/cart/${cartId}`);
    sleep(1);
    http.del(`${host}/cart/${cartId}/items/${productId}`, null, params);
    sleep(1);
    http.get(`${host}/cart/${cartId}`);
}