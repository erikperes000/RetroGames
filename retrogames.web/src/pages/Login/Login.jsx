import { useState } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import { useAutenticacao } from '../../hooks/useAutenticacao'

function Login() {
  const [email, setEmail] = useState('')
  const [senha, setSenha] = useState('')
  const [erro, setErro] = useState('')
  const [enviando, setEnviando] = useState(false)

  const [parametros] = useSearchParams()
  const navegar = useNavigate()
  const { entrar } = useAutenticacao()

  const voltarPara = parametros.get('voltarPara')
  const destino = voltarPara?.startsWith('/') && !voltarPara.startsWith('//')
    ? voltarPara
    : '/'

  async function tratarEnvio(evento) {
    evento.preventDefault()
    setErro('')
    setEnviando(true)

    try {
      await entrar(email, senha)
      navegar(destino, { replace: true })
    } catch (erro) {
      setErro(
        erro.response?.data?.errorMessage ??
        'Não foi possível realizar o login.'
      )
    } finally {
      setEnviando(false)
    }
  }

  return (
    <div className="row justify-content-center">
      <div className="col-md-6 col-lg-4">
        <div className="card shadow-sm">
          <div className="card-body p-4">
            <h2 className="text-center mb-4">Entrar</h2>

            {erro && (
              <div className="alert alert-danger" role="alert">
                {erro}
              </div>
            )}

            <form onSubmit={tratarEnvio}>
              <div className="mb-3">
                <label htmlFor="email" className="form-label">
                  E-mail
                </label>

                <input
                  id="email"
                  type="email"
                  className="form-control"
                  value={email}
                  onChange={(evento) => setEmail(evento.target.value)}
                  required
                  autoComplete="username"
                />
              </div>

              <div className="mb-3">
                <label htmlFor="senha" className="form-label">
                  Senha
                </label>

                <input
                  id="senha"
                  type="password"
                  className="form-control"
                  value={senha}
                  onChange={(evento) => setSenha(evento.target.value)}
                  required
                  autoComplete="current-password"
                />
              </div>

              <button
                type="submit"
                className="btn btn-primary w-100"
                disabled={enviando}
              >
                {enviando ? 'Entrando...' : 'Entrar'}
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  )
}

export default Login