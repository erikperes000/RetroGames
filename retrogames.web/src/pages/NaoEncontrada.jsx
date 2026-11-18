import { Link } from 'react-router-dom'

function NaoEncontrada() {
  return (
    <section className="text-center py-5">
      <h1> Página não encontrada </h1>

      <p>
        A página que você está procurando não existe.
      </p>

      <Link to="/" className="btn btn-primary">
        Voltar ao catálogo
      </Link>
    </section>
  )
}

export default NaoEncontrada