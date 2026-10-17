import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Error from './Pages/Error';
import Home from './Pages/Home';


function RoutesApp() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path='/' element={<Home />} />
                <Route path='*' element={<Error />} />
            </Routes>

        </BrowserRouter>
    )
}

export default RoutesApp;
