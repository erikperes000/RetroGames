import api from './api'

export const jogoApi = {
  listar: async (parametros = {}) => {
    const resposta = await api.get('/Jogo', {
      params: parametros
    })

    return resposta.data.data
  },

  buscarPorId: async (id) => {
    const resposta = await api.get(`/Jogo/${id}`)

    return resposta.data.data
  },

  inserir: async (jogo) => {
    const resposta = await api.post('/Jogo', jogo)

    return resposta.data.data
  },

  alterar: async (id, jogo) => {
    const resposta = await api.put(`/Jogo/${id}`, jogo)

    return resposta.data.data
  },

  desativar: async (id) => {
    const resposta = await api.delete(`/Jogo/${id}`)

    return resposta.data.data
  }
}

export const plataformaApi = {
  listar: async () => {
    const resposta = await api.get('/Plataforma')

    return resposta.data.data
  },

  inserir: async (plataforma) => {
    const resposta = await api.post('/Plataforma', plataforma)

    return resposta.data.data
  },

  alterar: async (id, plataforma) => {
    const resposta = await api.put(`/Plataforma/${id}`, plataforma)

    return resposta.data.data
  },

  excluir: async (id) => {
    const resposta = await api.delete(`/Plataforma/${id}`)

    return resposta.data.data
  }
}

export const generoApi = {
  listar: async () => {
    const resposta = await api.get('/Genero')

    return resposta.data.data
  }
}

export const condicaoJogoApi = {
  listar: async () => {
    const resposta = await api.get('/CondicaoJogo')

    return resposta.data.data
  }
}

export const autenticacaoApi = {
  entrar: async (email, senha) => {
    const resposta = await api.post('/Autenticacao/login', {
      email,
      senha
    })

    return resposta.data.data
  },

  sair: async () => {
    const resposta = await api.post('/Autenticacao/logout')

    return resposta.data.data
  },

  usuarioAtual: async () => {
    const resposta = await api.get('/Autenticacao/eu')

    return resposta.data.data
  }
}