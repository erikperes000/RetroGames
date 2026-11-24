import { useEffect, useState } from 'react'
import {
  Link,
  useNavigate,
  useParams
} from 'react-router-dom'

import {
  jogoApi,
  plataformaApi,
  generoApi,
  condicaoJogoApi
} from '../../api/servicos'

import {
  obterMensagemErro,
  obterErrosDosCampos
} from '../../api/erros'

import {
  Carregando,
  MensagemErro
} from '../../components/MensagensEstado'

function FormularioJogo() {
  const { id } = useParams()
  const navegar = useNavigate()

  const estaEditando = Boolean(id)

  const [nome, setNome] = useState('')
  const [descricao, setDescricao] = useState('')
  const [anoLancamento, setAnoLancamento] = useState('')
  const [preco, setPreco] = useState('')
  const [estoque, setEstoque] = useState('')
  const [idPlataforma, setIdPlataforma] = useState('')
  const [idCondicaoJogo, setIdCondicaoJogo] = useState('')
  const [idsGeneros, setIdsGeneros] = useState([])
  const [ativo, setAtivo] = useState(true)

  const [plataformas, setPlataformas] = useState([])
  const [generos, setGeneros] = useState([])
  const [condicoes, setCondicoes] = useState([])

  const [carregando, setCarregando] = useState(true)
  const [salvando, setSalvando] = useState(false)

  const [erro, setErro] = useState('')
  const [errosCampos, setErrosCampos] = useState({})

  useEffect(() => {
    async function carregarDados() {
      setCarregando(true)
      setErro('')

      try {
        const [
          listaPlataformas,
          listaGeneros,
          listaCondicoes
        ] = await Promise.all([
          plataformaApi.listar(),
          generoApi.listar(),
          condicaoJogoApi.listar()
        ])

        setPlataformas(listaPlataformas)
        setGeneros(listaGeneros)
        setCondicoes(listaCondicoes)

        if (estaEditando) {
          const jogo = await jogoApi.buscarPorId(id)

          setNome(jogo.nome)
          setDescricao(jogo.descricao || '')
          setAnoLancamento(String(jogo.anoLancamento))
          setPreco(String(jogo.preco))
          setEstoque(String(jogo.estoque))
          setIdPlataforma(String(jogo.idPlataforma))
          setIdCondicaoJogo(String(jogo.idCondicaoJogo))
          setIdsGeneros(
            jogo.generos.map(genero => genero.id)
          )
          setAtivo(jogo.ativo)
        }
      } catch (error) {
        setErro(obterMensagemErro(error))
      } finally {
        setCarregando(false)
      }
    }

    carregarDados()
  }, [id, estaEditando])
  
  function alternarGenero(idGenero) {
    if (idsGeneros.includes(idGenero)) {
      setIdsGeneros(idsGeneros.filter(
        valor => valor !== idGenero
      ))

      setErrosCampos(erros => ({
        ...erros,
        idsGeneros: ''
      }))

      return
    }

    if (idsGeneros.length >= 2) {
      setErrosCampos(erros => ({
        ...erros,
        idsGeneros: 'Você pode selecionar no máximo dois gêneros.'
      }))

      return
    }

    setIdsGeneros([...idsGeneros, idGenero])

    setErrosCampos(erros => ({
      ...erros,
      idsGeneros: ''
    }))
  }
  
  async function salvarJogo(evento) {
    evento.preventDefault()

    setErro('')
    setErrosCampos({})

    const novosErros = {}

    // Validação do preço
    if (preco === '' || Number(preco) <= 0) {
      novosErros.preco = 'O preço deve ser maior que zero.'
    }

    // Validação dos gêneros
    if (idsGeneros.length === 0) {
      novosErros.idsGeneros = 'Selecione pelo menos um gênero.'
    } else if (idsGeneros.length > 2) {
      novosErros.idsGeneros = 'Selecione no máximo dois gêneros.'
    }

    // Interrompe o cadastro se houver erros
    if (Object.keys(novosErros).length > 0) {
      setErrosCampos(novosErros)
      return
    }

    const dados = {
      nome: nome.trim(),
      descricao: descricao.trim() || null,
      anoLancamento: Number(anoLancamento),
      preco: Number(preco),
      estoque: Number(estoque),
      idPlataforma: Number(idPlataforma),
      idCondicaoJogo: Number(idCondicaoJogo),
      idsGeneros: idsGeneros.map(Number)
    }

    if (estaEditando) {
      dados.ativo = ativo
    }

    setSalvando(true)

    try {
      if (estaEditando) {
        await jogoApi.alterar(id, dados)
      } else {
        await jogoApi.inserir(dados)
      }

      navegar('/admin/jogos')
    } catch (error) {
      setErrosCampos(obterErrosDosCampos(error))
      setErro(obterMensagemErro(error))
    } finally {
      setSalvando(false)
    }
  }

  if (carregando) {
    return <Carregando texto="Carregando formulário..." />
  }

  return (
    <section>
      <div className="mb-4">
        <h1>
          {estaEditando ? 'Editar jogo' : 'Novo jogo'}
        </h1>

        <p className="text-secondary">
          Preencha os dados do jogo.
        </p>
      </div>

      {erro && (
        <MensagemErro mensagem={erro} />
      )}

      <div className="card">
        <div className="card-body">
          <form onSubmit={salvarJogo}>
            <div className="row g-3">

              <div className="col-12">
                <label htmlFor="nome" className="form-label">
                  Nome
                </label>

                <input
                  id="nome"
                  type="text"
                  maxLength="150"
                  required
                  className={`form-control ${errosCampos.nome ? 'is-invalid' : ''}`}
                  value={nome}
                  onChange={evento => setNome(evento.target.value)}
                />

                {errosCampos.nome && (
                  <div className="invalid-feedback">
                    {errosCampos.nome}
                  </div>
                )}
              </div>

              <div className="col-12">
                <label htmlFor="descricao" className="form-label">
                  Descrição
                </label>

                <textarea
                  id="descricao"
                  rows="3"
                  maxLength="1000"
                  className={`form-control ${errosCampos.descricao ? 'is-invalid' : ''}`}
                  value={descricao}
                  onChange={evento => setDescricao(evento.target.value)}
                />

                {errosCampos.descricao && (
                  <div className="invalid-feedback">
                    {errosCampos.descricao}
                  </div>
                )}
              </div>

              <div className="col-12 col-md-4">
                <label htmlFor="anoLancamento" className="form-label">
                  Ano de lançamento
                </label>

                <input
                  id="anoLancamento"
                  type="number"
                  min="1970"
                  max="2100"
                  required
                  className={`form-control ${errosCampos.anoLancamento ? 'is-invalid' : ''}`}
                  value={anoLancamento}
                  onChange={evento => setAnoLancamento(evento.target.value)}
                />

                {errosCampos.anoLancamento && (
                  <div className="invalid-feedback">
                    {errosCampos.anoLancamento}
                  </div>
                )}
              </div>

              <div className="col-12 col-md-4">
                <label htmlFor="preco" className="form-label">
                  Preço
                </label>

                <input
                  id="preco"
                  type="number"
                  min="0"
                  step="0.01"
                  required
                  className={`form-control ${
                    errosCampos.preco ? 'is-invalid' : ''
                  }`}
                  value={preco}
                  onChange={evento => {
                    setPreco(evento.target.value)

                    setErrosCampos(erros => ({
                      ...erros,
                      preco: ''
                    }))
                  }}
                />

                {errosCampos.preco && (
                  <div className="invalid-feedback">
                    {errosCampos.preco}
                  </div>
                )}

                {errosCampos.preco && (
                  <div className="invalid-feedback">
                    {errosCampos.preco}
                  </div>
                )}
              </div>

              <div className="col-12 col-md-4">
                <label htmlFor="estoque" className="form-label">
                  Estoque
                </label>

                <input
                  id="estoque"
                  type="number"
                  min="0"
                  step="1"
                  required
                  className={`form-control ${errosCampos.estoque ? 'is-invalid' : ''}`}
                  value={estoque}
                  onChange={evento => setEstoque(evento.target.value)}
                />

                {errosCampos.estoque && (
                  <div className="invalid-feedback">
                    {errosCampos.estoque}
                  </div>
                )}
              </div>

              <div className="col-12 col-md-6">
                <label htmlFor="idPlataforma" className="form-label">
                  Plataforma
                </label>

                <select
                  id="idPlataforma"
                  required
                  className={`form-select ${errosCampos.idPlataforma ? 'is-invalid' : ''}`}
                  value={idPlataforma}
                  onChange={evento => setIdPlataforma(evento.target.value)}
                >
                  <option value="">Selecione uma plataforma</option>

                  {plataformas.map(plataforma => (
                    <option
                      key={plataforma.id}
                      value={plataforma.id}
                    >
                      {plataforma.nome}
                    </option>
                  ))}
                </select>

                {errosCampos.idPlataforma && (
                  <div className="invalid-feedback">
                    {errosCampos.idPlataforma}
                  </div>
                )}
              </div>

              <div className="col-12 col-md-6">
                <label htmlFor="idCondicaoJogo" className="form-label">
                  Condição do jogo
                </label>

                <select
                  id="idCondicaoJogo"
                  required
                  className={`form-select ${errosCampos.idCondicaoJogo ? 'is-invalid' : ''}`}
                  value={idCondicaoJogo}
                  onChange={evento => setIdCondicaoJogo(evento.target.value)}
                >
                  <option value="">Selecione uma condição</option>

                  {condicoes.map(condicao => (
                    <option
                      key={condicao.id}
                      value={condicao.id}
                    >
                      {condicao.descricao}
                    </option>
                  ))}
                </select>

                {errosCampos.idCondicaoJogo && (
                  <div className="invalid-feedback">
                    {errosCampos.idCondicaoJogo}
                  </div>
                )}
              </div>

              <div className="col-12">
                <label className="form-label">
                  Gêneros
                </label>

                <div className={`border rounded p-3 ${errosCampos.idsGeneros ? 'border-danger' : ''}`}>
                  <div className="row">
                    {generos.map(genero => (
                      <div
                        className="col-12 col-sm-6 col-md-4"
                        key={genero.id}
                      >
                        <div className="form-check">
                          <input
                            id={`genero-${genero.id}`}
                            type="checkbox"
                            className="form-check-input"
                            checked={idsGeneros.includes(genero.id)}
                            disabled={
                              idsGeneros.length >= 2 &&
                              !idsGeneros.includes(genero.id)
                            }
                            onChange={() => alternarGenero(genero.id)}
                          />

                          <label
                            className="form-check-label"
                            htmlFor={`genero-${genero.id}`}
                          >
                            {genero.nome}
                          </label>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>

                {errosCampos.idsGeneros && (
                  <div className="text-danger small mt-1">
                    {errosCampos.idsGeneros}
                  </div>
                )}
              </div>

              {estaEditando && (
                <div className="col-12">
                  <div className="form-check">
                    <input
                      id="ativo"
                      type="checkbox"
                      className="form-check-input"
                      checked={ativo}
                      onChange={evento => setAtivo(evento.target.checked)}
                    />

                    <label
                      htmlFor="ativo"
                      className="form-check-label"
                    >
                      Jogo ativo
                    </label>
                  </div>
                </div>
              )}

            </div>

            <div className="d-flex gap-2 mt-4">
              <button
                type="submit"
                className="btn btn-primary"
                disabled={salvando}
              >
                {salvando ? 'Salvando...' : 'Salvar'}
              </button>

              <Link
                to="/admin/jogos"
                className="btn btn-secondary"
              >
                Cancelar
              </Link>
            </div>

          </form>
        </div>
      </div>
    </section>
  )
}

export default FormularioJogo