# ecosim

Regex pt extragere date din loguri

(\d+)\s+\d+\s+0\s+2\s+([+\-]\d+(\.\d+)?) - selecteaza intrarile pt mediere, si suma castigata din ea
(\d+)\s+\d+\s+0\s+1\s+([+\-]\d+(\.\d+)?) - selecteaza intrarile pt vanzare, si suma castigata din ea
(\d+)\s+\d+\s+0\s+0\s+([+\-]\d+(\.\d+)?) - selecteaza intrarile pt cumparare, si suma pierduta din ea

intrarile selecteaza id-ul nodului in cauza
