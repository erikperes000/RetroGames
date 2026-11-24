import { Link, Outlet, useNavigate } from 'react-router-dom'
import { useAutenticacao } from '../hooks/useAutenticacao'

function Layout() {
  const {
    usuario,
    carregando,
    estaAutenticado,
    ehAdministrador,
    sair
  } = useAutenticacao()

  const navegar = useNavigate()

  async function tratarSaida() {
    try {
      await sair()
    } finally {
      navegar('/', { replace: true })
    }
  }

  return (
    <>
      <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
        <div className="container">
          <Link className="navbar-brand" to="/">
            RetroGames
          </Link>

          <button
            className="navbar-toggler"
            type="button"
            data-bs-toggle="collapse"
            data-bs-target="#menuPrincipal"
            aria-controls="menuPrincipal"
            aria-expanded="false"
            aria-label="Abrir menu"
          >
            <span className="navbar-toggler-icon"></span>
          </button>

          <div
            className="collapse navbar-collapse"
            id="menuPrincipal"
          >
            <div className="navbar-nav me-auto">
              <Link className="nav-link" to="/">
                Catálogo
              </Link>

              {ehAdministrador && (
                <div className="nav-item dropdown">
                  <button
                    className="nav-link dropdown-toggle btn btn-link"
                    type="button"
                    data-bs-toggle="dropdown"
                    aria-expanded="false"
                  >
                    Administração
                  </button>

                  <ul className="dropdown-menu">
                    <li>
                      <Link
                        className="dropdown-item"
                        to="/admin/jogos"
                      >
                        Jogos
                      </Link>
                    </li>

                    <li>
                      <Link
                        className="dropdown-item"
                        to="/admin/plataformas"
                      >
                        Plataformas
                      </Link>
                    </li>
                  </ul>
                </div>
              )}
            </div>

            <div className="navbar-nav">
              {!carregando && (
                estaAutenticado ? (
                  <>
                    <span className="nav-link">
                      Olá, {usuario.nome}
                    </span>

                    <button
                      className="nav-link btn btn-link"
                      type="button"
                      onClick={tratarSaida}
                    >
                      Sair
                    </button>
                  </>
                ) : (
                  <Link className="nav-link" to="/login">
                    Entrar
                  </Link>
                )
              )}
            </div>
          </div>
        </div>
      </nav>

      <main className="container py-4">
        <Outlet />
      </main>

      <footer className="bg-dark text-white text-center py-3 mt-5">
        <div className="container">
          RetroGames - Loja de jogos retrô
        </div>
      </footer>
    </>
  )
}

export default Layout