import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'

import {
  jogoApi,
  plataformaApi,
  generoApi
} from '../../api/servicos'

import { obterMensagemErro } from '../../api/erros'
import CardJogo from '../../components/CardJogo'
import Paginacao from '../../components/Paginacao'

import {
  Carregando,
  MensagemErro,
  EstadoVazio
} from '../../components/MensagensEstado'

function Catalogo() {
  const [searchParams, setSearchParams] = useSearchParams()

  const busca = searchParams.get('busca') || ''
  const idPlataforma = searchParams.get('idPlataforma') || ''
  const idGenero = searchParams.get('idGenero') || ''
  const pagina = Number(searchParams.get('pagina')) || 1

  const [jogos, setJogos] = useState([])
  const [totalPaginas, setTotalPaginas] = useState(1)

  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])

  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState('')
  const [tentativa, setTentativa] = useState(0)

  // Carrega as opções dos filtros uma vez.
  useEffect(() => {
    async function carregarFiltros() {
      try {
        const [listaPlataformas, listaGeneros] = await Promise.all([
          plataformaApi.listar(),
          generoApi.listar()
        ])

        setPlataformas(listaPlataformas)
        setGeneros(listaGeneros)
      } catch (error) {
        setErro(obterMensagemErro(error))
      }
    }

    carregarFiltros()
  }, [tentativa])

  // Busca os jogos sempre que um filtro ou a página mudar.
  useEffect(() => {
    let ativo = true

    async function carregarJogos() {
      setCarregando(true)
      setErro('')

      try {
        const resultado = await jogoApi.listar({
          busca,
          idPlataforma,
          idGenero,
          pagina,
          tamanhoPagina: 12
        })

        if (ativo) {
          setJogos(resultado.itens)
          setTotalPaginas(resultado.totalPaginas)
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

    carregarJogos()

    return () => {
      ativo = false
    }
  }, [busca, idPlataforma, idGenero, pagina, tentativa])

  // Atualiza um filtro na URL e volta para a primeira página.
  function atualizarFiltro(nome, valor) {
    const novosParametros = new URLSearchParams(searchParams)

    if (valor) {
      novosParametros.set(nome, valor)
    } else {
      novosParametros.delete(nome)
    }

    novosParametros.delete('pagina')
    setSearchParams(novosParametros)
  }

  // Altera somente a página atual.
  function mudarPagina(novaPagina) {
    const novosParametros = new URLSearchParams(searchParams)

    if (novaPagina <= 1) {
      novosParametros.delete('pagina')
    } else {
      novosParametros.set('pagina', String(novaPagina))
    }

    setSearchParams(novosParametros)
  }

  function tentarNovamente() {
    setTentativa(valor => valor + 1)
  }

  return (
    <section>
      <div className="mb-4">
        <h1>Catálogo de jogos</h1>
        <p className="text-secondary">
          Encontre seus jogos retrô favoritos.
        </p>
      </div>

      <div className="row g-3 mb-4">
        <div className="col-12 col-md-4">
          <label htmlFor="busca" className="form-label">
            Buscar por nome
          </label>

          <input
            id="busca"
            type="search"
            className="form-control"
            placeholder="Digite o nome do jogo"
            value={busca}
            onChange={evento =>
              atualizarFiltro('busca', evento.target.value)
            }
          />
        </div>

        <div className="col-12 col-md-4">
          <label htmlFor="plataforma" className="form-label">
            Plataforma
          </label>

          <select
            id="plataforma"
            className="form-select"
            value={idPlataforma}
            onChange={evento =>
              atualizarFiltro('idPlataforma', evento.target.value)
            }
          >
            <option value="">Todas as plataformas</option>

            {plataformas.map(plataforma => (
              <option key={plataforma.id} value={plataforma.id}>
                {plataforma.nome}
              </option>
            ))}
          </select>
        </div>

        <div className="col-12 col-md-4">
          <label htmlFor="genero" className="form-label">
            Gênero
          </label>

          <select
            id="genero"
            className="form-select"
            value={idGenero}
            onChange={evento =>
              atualizarFiltro('idGenero', evento.target.value)
            }
          >
            <option value="">Todos os gêneros</option>

            {generos.map(genero => (
              <option key={genero.id} value={genero.id}>
                {genero.nome}
              </option>
            ))}
          </select>
        </div>
      </div>

      {erro && (
        <MensagemErro
          mensagem={erro}
          aoTentarNovamente={tentarNovamente}
        />
      )}

      {carregando && <Carregando texto="Carregando jogos..." />}

      {!carregando && !erro && jogos.length === 0 && (
        <EstadoVazio titulo="Nenhum jogo encontrado.">
          Tente alterar a busca ou os filtros.
        </EstadoVazio>
      )}

      {!carregando && !erro && jogos.length > 0 && (
        <>
          <div className="row g-4">
            {jogos.map(jogo => (
              <div className="col-12 col-sm-6 col-lg-4" key={jogo.id}>
                <CardJogo jogo={jogo} />
              </div>
            ))}
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

export default Catalogo