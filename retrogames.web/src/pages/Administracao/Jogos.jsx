import { useCallback, useEffect, useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'

import { jogoApi } from '../../api/servicos'
import { obterMensagemErro } from '../../api/erros'

import Paginacao from '../../components/Paginacao'

import {
  Carregando,
  MensagemErro,
  EstadoVazio
} from '../../components/MensagensEstado'

import { formatarPreco } from '../../lib/formato'

function Jogos() {
  const [parametros, setParametros] = useSearchParams()

  const busca = parametros.get('busca') || ''
  const pagina = Number(parametros.get('pagina')) || 1

  const [jogos, setJogos] = useState([])
  const [totalPaginas, setTotalPaginas] = useState(1)

  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const [tentativa, setTentativa] = useState(0)

  const carregarJogos = useCallback(async () => {
    setCarregando(true)
    setErro('')

    try {
      const resultado = await jogoApi.listar({
        busca,
        pagina,
        tamanhoPagina: 12,
        incluirInativos: true
      })

      setJogos(resultado.itens)
      setTotalPaginas(resultado.totalPaginas)
    } catch (error) {
      setErro(obterMensagemErro(error))
    } finally {
      setCarregando(false)
    }
  }, [busca, pagina])

  useEffect(() => {
    carregarJogos()
  }, [carregarJogos, tentativa])

  function atualizarBusca(valor) {
    const novosParametros = new URLSearchParams(parametros)

    if (valor) {
      novosParametros.set('busca', valor)
    } else {
      novosParametros.delete('busca')
    }

    novosParametros.delete('pagina')
    setParametros(novosParametros)
  }

  function mudarPagina(novaPagina) {
    const novosParametros = new URLSearchParams(parametros)

    if (novaPagina <= 1) {
      novosParametros.delete('pagina')
    } else {
      novosParametros.set('pagina', String(novaPagina))
    }

    setParametros(novosParametros)
  }

  async function desativarJogo(jogo) {
    const confirmou = window.confirm(
      `Deseja realmente desativar o jogo "${jogo.nome}"?`
    )

    if (!confirmou) {
      return
    }

    setErro('')

    try {
      await jogoApi.desativar(jogo.id)
      await carregarJogos()
    } catch (error) {
      setErro(obterMensagemErro(error))
    }
  }

  function tentarNovamente() {
    setTentativa(valor => valor + 1)
  }

  return (
    <section>
      <div className="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
        <div>
          <h1>Jogos</h1>

          <p className="text-secondary">
            Gerencie os jogos cadastrados na loja.
          </p>
        </div>

        <Link
          to="/admin/jogos/novo"
          className="btn btn-primary"
        >
          Novo jogo
        </Link>
      </div>

      <div className="mb-4">
        <label htmlFor="buscaJogos" className="form-label">
          Buscar jogo
        </label>

        <input
          id="buscaJogos"
          type="search"
          className="form-control"
          placeholder="Digite o nome do jogo"
          value={busca}
          onChange={evento => atualizarBusca(evento.target.value)}
        />
      </div>

      {erro && (
        <MensagemErro
          mensagem={erro}
          aoTentarNovamente={tentarNovamente}
        />
      )}

      {carregando && (
        <Carregando texto="Carregando jogos..." />
      )}

      {!carregando && !erro && jogos.length === 0 && (
        <EstadoVazio titulo="Nenhum jogo encontrado.">
          Cadastre um novo jogo ou altere sua pesquisa.
        </EstadoVazio>
      )}

      {!carregando && !erro && jogos.length > 0 && (
        <>
          <div className="table-responsive">
            <table className="table table-striped table-hover align-middle">
              <thead>
                <tr>
                  <th>Nome</th>
                  <th>Plataforma</th>
                  <th>Ano</th>
                  <th>Preço</th>
                  <th>Estoque</th>
                  <th>Situação</th>
                  <th>Ações</th>
                </tr>
              </thead>

              <tbody>
                {jogos.map(jogo => (
                  <tr key={jogo.id}>
                    <td>{jogo.nome}</td>

                    <td>{jogo.nomePlataforma}</td>

                    <td>{jogo.anoLancamento}</td>

                    <td>{formatarPreco(jogo.preco)}</td>

                    <td>{jogo.estoque}</td>

                    <td>
                      {jogo.ativo ? (
                        <span className="badge text-bg-success">
                          Ativo
                        </span>
                      ) : (
                        <span className="badge text-bg-secondary">
                          Inativo
                        </span>
                      )}
                    </td>

                    <td>
                      <div className="d-flex gap-2">
                        <Link
                          to={`/admin/jogos/${jogo.id}/editar`}
                          className="btn btn-sm btn-outline-primary"
                        >
                          Editar
                        </Link>

                        {jogo.ativo && (
                          <button
                            type="button"
                            className="btn btn-sm btn-outline-danger"
                            onClick={() => desativarJogo(jogo)}
                          >
                            Desativar
                          </button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="mt-4">
            <Paginacao
              pagina={pagina}
              totalPaginas={totalPaginas}
              aoMudar={mudarPagina}
            />
          </div>
        </>
      )}
    </section>
  )
}

export default Jogos