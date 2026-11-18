import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { jogoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'
import { formatarPreco } from '../../lib/formato'
import { Carregando, MensagemErro } from '../../components/MensagensEstado'

function DetalheJogo() {
  const { id } = useParams()
  const [jogo, setJogo] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const [tentativa, setTentativa] = useState(0)

  useEffect(() => {
    let ativo = true

    async function carregarJogo() {
      setCarregando(true)
      setErro('')
      setJogo(null)

      try {
        const resultado = await jogoApi.buscarPorId(id)

        if (ativo) {
          setJogo(resultado)
        }
      } catch (error) {
        if (ativo) {
          setErro(obterMensagemErro(error))
        }
      } finally {
        if (ativo) {
          setCarregando(false)
        }
      }
    }

    carregarJogo()

    return () => {
      ativo = false
    }
  }, [id, tentativa])

  function tentarNovamente() {
    setTentativa(valor => valor + 1)
  }

  if (carregando) {
    return <Carregando texto="Carregando detalhes do jogo..." />
  }

  if (erro) {
    return (
      <div>
        <MensagemErro
          mensagem={erro}
          aoTentarNovamente={tentarNovamente}
        />
        <Link to="/" className="btn btn-outline-primary mt-3">
          Voltar ao catálogo
        </Link>
      </div>
    )
  }

  if (!jogo) {
    return null
  }

  return (
    <section>
      <Link to="/" className="btn btn-outline-secondary mb-4">
        &larr; Voltar ao catálogo
      </Link>

      <div className="card shadow-sm">
        <div className="card-body p-4">
          <div className="d-flex flex-wrap justify-content-between align-items-start gap-2 mb-3">
            <h1 className="h2 mb-0">{jogo.nome}</h1>

            {jogo.estoque <= 0 && (
              <span className="badge text-bg-danger">
                Esgotado
              </span>
            )}
          </div>

          <p className="text-secondary mb-2">
            Plataforma: {jogo.nomePlataforma}
          </p>

          <p className="mb-2">
            <strong>Condição:</strong> {jogo.descricaoCondicao}
          </p>

          <p className="mb-2">
            <strong>Ano de lançamento:</strong> {jogo.anoLancamento}
          </p>

          <p className="mb-2">
            <strong>Gêneros:</strong> {jogo.generos?.join(', ')}
          </p>

          {jogo.descricao && (
            <div className="mb-3">
              <h2 className="h5">Descrição</h2>
              <p>{jogo.descricao}</p>
            </div>
          )}

          <p className="mb-2">
            <strong>Estoque:</strong> {jogo.estoque}
          </p>

          <p className="fs-3 fw-bold text-primary mb-0">
            {formatarPreco(jogo.preco)}
          </p>
        </div>
      </div>
    </section>
  )
}

export default DetalheJogo