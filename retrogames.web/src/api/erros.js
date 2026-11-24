export function obterMensagemErro(erro) {
  if (!erro.response) {
    return 'Não foi possível conectar ao servidor.'
  }

  return (
    erro.response.data?.errorMessage ||
    'Ocorreu um erro ao realizar a operação.'
  )
}

export function obterErrosDosCampos(erro) {
  const resposta = erro.response?.data

  if (resposta?.responseCode !== 3 || !resposta.data) {
    return {}
  }

  return Object.fromEntries(
    Object.entries(resposta.data).map(([campo, mensagens]) => [
      campo,
      Array.isArray(mensagens) ? mensagens[0] : mensagens
    ])
  )
}