import { useEffect, useState } from 'react'

import { plataformaApi } from '../../api/servicos'
import {
  obterMensagemErro,
  obterErrosDosCampos
} from '../../api/erros'

import {
  Carregando,
  MensagemErro,
  EstadoVazio
} from '../../components/MensagensEstado'

function Plataforma() {
  //---------
  const [erroCarregamento, setErroCarregamento] = useState('')
  //---------

  const [plataformas, setPlataformas] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [salvando, setSalvando] = useState(false)

  const [erro, setErro] = useState('')
  const [errosCampos, setErrosCampos] = useState({})

  const [plataformaEmEdicao, setPlataformaEmEdicao] = useState(null)

  const [nome, setNome] = useState('')
  const [fabricante, setFabricante] = useState('')
  const [anoLancamento, setAnoLancamento] = useState('')

  const [tentativa, setTentativa] = useState(0)

  async function carregarPlataformas() {
    setCarregando(true)
    //setErro('')
    setErroCarregamento('')

    try {
      const resultado = await plataformaApi.listar()
      setPlataformas(resultado)
    } catch (error) {
      //setErro(obterMensagemErro(error))
      setErroCarregamento(obterMensagemErro(error))
    } finally {
      setCarregando(false)
    }
  }

  useEffect(() => {
    carregarPlataformas()
  }, [tentativa])

  function limparFormulario() {
    setPlataformaEmEdicao(null)
    setNome('')
    setFabricante('')
    setAnoLancamento('')
    setErrosCampos({})
    setErro('')
  }

  function editarPlataforma(plataforma) {
    setPlataformaEmEdicao(plataforma)
    setNome(plataforma.nome)
    setFabricante(plataforma.fabricante)
    setAnoLancamento(String(plataforma.anoLancamento))
    setErrosCampos({})
    setErro('')

    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  async function salvarPlataforma(evento) {
    evento.preventDefault()

    if (!nome.trim()) {
      setErrosCampos({ nome: 'Informe o nome.' })
      return
    }

    setSalvando(true)
    setErro('')
    setErrosCampos({})

    const dados = {
      nome,
      fabricante,
      anoLancamento: Number(anoLancamento)
    }

    try {
      if (plataformaEmEdicao) {
        await plataformaApi.alterar(plataformaEmEdicao.id, dados)
      } else {
        await plataformaApi.inserir(dados)
      }

      limparFormulario()
      await carregarPlataformas()
    } catch (error) {
      setErrosCampos(obterErrosDosCampos(error))
      setErro(obterMensagemErro(error))
    } finally {
      setSalvando(false)
    }
  }

  async function excluirPlataforma(plataforma) {
    const confirmou = window.confirm(
      `Deseja realmente excluir a plataforma "${plataforma.nome}"?`
    )

    if (!confirmou) {
      return
    }

    setErro('')

    try {
      await plataformaApi.excluir(plataforma.id)
      await carregarPlataformas()
    } catch (error) {
      setErro(obterMensagemErro(error))
    }
  }

  function tentarNovamente() {
    setTentativa(valor => valor + 1)
  }

  return (
    <section>
      <div className="mb-4">
        <h1>Plataformas</h1>
        <p className="text-secondary">
          Gerencie as plataformas disponíveis na loja.
        </p>
      </div>

      <div className="card mb-4">
        <div className="card-header">
          {plataformaEmEdicao
            ? 'Editar plataforma'
            : 'Nova plataforma'}
        </div>

        <div className="card-body">
          <form onSubmit={salvarPlataforma}>
            <div className="row g-3">
              <div className="col-12 col-md-4">
                <label htmlFor="nome" className="form-label">
                  Nome
                </label>

                <input
                  id="nome"
                  type="text"
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

              <div className="col-12 col-md-4">
                <label htmlFor="fabricante" className="form-label">
                  Fabricante
                </label>

                <input
                  id="fabricante"
                  type="text"
                  className={`form-control ${errosCampos.fabricante ? 'is-invalid' : ''}`}
                  value={fabricante}
                  onChange={evento => setFabricante(evento.target.value)}
                />

                {errosCampos.fabricante && (
                  <div className="invalid-feedback">
                    {errosCampos.fabricante}
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
                  required
                  step="1"
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
            </div>

            {erro && (
              <div className="alert alert-danger mt-3 mb-0">
                {erro}
              </div>
            )}

            <div className="mt-3 d-flex gap-2">
              <button
                type="submit"
                className="btn btn-primary"
                disabled={salvando}
              >
                {salvando ? 'Salvando...' : 'Salvar'}
              </button>

              {plataformaEmEdicao && (
                <button
                  type="button"
                  className="btn btn-secondary"
                  onClick={limparFormulario}
                >
                  Cancelar edição
                </button>
              )}
            </div>
          </form>
        </div>
      </div>

      {erroCarregamento && (
        <MensagemErro
          mensagem={erroCarregamento}
          aoTentarNovamente={tentarNovamente}
        />
      )}

      {carregando && (
        <Carregando texto="Carregando plataformas..." />
      )}

      {!carregando && !erro && plataformas.length === 0 && (
        <EstadoVazio titulo="Nenhuma plataforma cadastrada.">
          Utilize o formulário acima para cadastrar uma plataforma.
        </EstadoVazio>
      )}

      {!carregando && plataformas.length > 0 && (
        <div className="table-responsive">
          <table className="table table-striped table-hover align-middle">
            <thead>
              <tr>
                <th>Nome</th>
                <th>Fabricante</th>
                <th>Ano de lançamento</th>
                <th className="text-end">Ações</th>
              </tr>
            </thead>

            <tbody>
              {plataformas.map(plataforma => (
                <tr key={plataforma.id}>
                  <td>{plataforma.nome}</td>
                  <td>{plataforma.fabricante}</td>
                  <td>{plataforma.anoLancamento}</td>
                  <td className="text-end">
                    <div className="d-flex justify-content-end gap-2">
                      <button
                        type="button"
                        className="btn btn-sm btn-outline-primary"
                        onClick={() => editarPlataforma(plataforma)}
                      >
                        Editar
                      </button>

                      <button
                        type="button"
                        className="btn btn-sm btn-outline-danger"
                        onClick={() => excluirPlataforma(plataforma)}
                      >
                        Excluir
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  )
}

export default Plataforma