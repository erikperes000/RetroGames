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
  }
}

export const plataformaApi = {
  listar: async () => {
    const resposta = await api.get('/Plataforma')

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