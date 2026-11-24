import { useCallback, useEffect, useState } from 'react';

import AutenticacaoContext from './autenticacaoContext';
import { autenticacaoApi } from '../api/servicos';

function AutenticacaoProvider({ children }) {
    const [usuario, setUsuario] = useState(null);
    const [carregando, setCarregando] = useState(true);

    const usuarioAtual = useCallback(async () => {
        try {
            const dados = await autenticacaoApi.usuarioAtual();
            setUsuario(dados);
        } catch (erro) {
            if (erro.response?.status === 401) {
                setUsuario(null);
            } else {
                console.error('Erro ao consultar usuário:', erro);
                setUsuario(null);
            }
        } finally {
            setCarregando(false);
        }
    }, []);

    useEffect(() => {
        usuarioAtual();

        const tratarSessaoExpirada = () => {
            setUsuario(null);
            setCarregando(false);
        };

        window.addEventListener(
            'sessao:expirada',
            tratarSessaoExpirada
        );

        return () => {
            window.removeEventListener(
                'sessao:expirada',
                tratarSessaoExpirada
            );
        };
    }, [usuarioAtual]);

    const entrar = async (email, senha) => {
        await autenticacaoApi.entrar(email, senha);
        await usuarioAtual();
    };

    const sair = async () => {
        try {
            await autenticacaoApi.sair();
        } finally {
            setUsuario(null);
        }
    };

    const estaAutenticado = usuario !== null;

    const ehAdministrador =
        usuario?.perfil === 'Administrador';

    return (
        <AutenticacaoContext.Provider
            value={{
                usuario,
                carregando,
                estaAutenticado,
                ehAdministrador,
                entrar,
                sair
            }}
        >
            {children}
        </AutenticacaoContext.Provider>
    );
}

export default AutenticacaoProvider;