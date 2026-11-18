import { Link, Outlet } from 'react-router-dom'

function Layout() {
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
            <div className="navbar-nav">
              <Link className="nav-link" to="/">
                Catálogo
              </Link>
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