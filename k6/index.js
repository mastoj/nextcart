import http from 'k6/http';
import { sleep } from 'k6';

export default function () {
    http.get('http://localhost:5095/cart/87dee802-c862-4069-936c-f41130460002');
}