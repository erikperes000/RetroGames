import { Routes, Route } from 'react-router-dom'
import Layout from './components/Layout'
import RotaProtegida from './components/RotaProtegida'
import Catalogo from './pages/Catalogo/Catalogo'
import DetalheJogo from './pages/Catalogo/DetalheJogo'
import Login from './pages/Login/Login'
import NaoEncontrada from './pages/NaoEncontrada'

function Jogos() {
  return <h1>Administração de Jogos</h1>
}

function NovoJogo() {
  return <h1>Novo Jogo</h1>
}

function EditarJogo() {
  return <h1>Editar Jogo</h1>
}

function Plataformas() {
  return <h1>Administração de Plataformas</h1>
}

function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<Catalogo />} />
        <Route path="jogos/:id" element={<DetalheJogo />} />
        <Route path="login" element={<Login />} />

        <Route
          path="admin"
          element={<RotaProtegida somenteAdministrador />}
        >
          <Route path="jogos" element={<Jogos />} />
          <Route path="jogos/novo" element={<NovoJogo />} />
          <Route path="jogos/:id/editar" element={<EditarJogo />} />
          <Route path="plataformas" element={<Plataformas />} />
        </Route>

        <Route path="*" element={<NaoEncontrada />} />
      </Route>
    </Routes>
  )
}

export default App