
import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAutenticacao } from '../hooks/useAutenticacao'

function RotaProtegida({ somenteAdministrador = false }) {
  const {
    carregando,
    estaAutenticado,
    ehAdministrador
  } = useAutenticacao()

  const localizacao = useLocation()

  if (carregando) {
    return <p className="container mt-4">Verificando sessão...</p>
  }

  if (!estaAutenticado) {
    const voltarPara = localizacao.pathname + localizacao.search

    return (
      <Navigate
        to={`/login?voltarPara=${encodeURIComponent(voltarPara)}`}
        replace
      />
    )
  }

  if (somenteAdministrador && !ehAdministrador) {
    return <Navigate to="/" replace />
  }

  return <Outlet />
}

export default RotaProtegida
