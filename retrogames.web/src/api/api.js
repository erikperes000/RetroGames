import axios from 'axios';

const api = axios.create({
    baseURL: '/api',
    withCredentials: true
});

api.interceptors.response.use(
    resposta => resposta,
    erro => {
        const status = erro.response?.status;
        const url = erro.config?.url ?? '';

        const ehAutenticacao =
            url.includes('/Autenticacao/eu') ||
            url.includes('/Autenticacao/login');

        if (status === 401 && !ehAutenticacao) {
            window.dispatchEvent(
                new Event('sessao:expirada')
            );
        }

        return Promise.reject(erro);
    }
);

export default api;