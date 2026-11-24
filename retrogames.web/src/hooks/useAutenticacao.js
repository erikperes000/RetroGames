import { useContext } from 'react';
import AutenticacaoContext from '../contexts/autenticacaoContext';

export function useAutenticacao() {
    const contexto = useContext(AutenticacaoContext);

    if (!contexto) {
        throw new Error(
            'useAutenticacao deve ser utilizado dentro de AutenticacaoProvider.'
        );
    }

    return contexto;
}