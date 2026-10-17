import { Link } from "react-router-dom";

function Home()
{
    return(
        <div>
             <h2>Pagina Home</h2>
             <Link to={"/dd"}>Nao encontrado</Link>

        </div>
       


    )
}

export default Home;