# Catalogo PDF - Base de Conocimiento

## Objetivo
Este modulo permite a un usuario con rol `Admin` generar un catalogo de productos, abrir una vista previa en una pestana nueva y desde ahi usar `Guardar como PDF` del navegador.

## Flujo actual
1. El acceso inicia desde `Pages/Productos/ProductosAdmin.cshtml`.
2. El boton `Catalogo PDF` abre `Pages/Productos/CatalogoPdf.cshtml`.
3. En esa vista se selecciona:
   - sucursal (`chulin` o `cardoza`)
   - filtro por nombre (`LIKE` sobre `Descripcion`)
   - si se incluyen todos los productos o solo los que tienen existencia
4. El formulario abre `Pages/Productos/CatalogoPdfPreview.cshtml` en una nueva pestana.
5. En la vista previa el usuario revisa el catalogo y usa el boton `Guardar como PDF`.

## Archivos involucrados
- `Pages/Productos/CatalogoPdf.cshtml`
- `Pages/Productos/CatalogoPdf.cshtml.cs`
- `Pages/Productos/CatalogoPdfPreview.cshtml`
- `Pages/Productos/CatalogoPdfPreview.cshtml.cs`
- `Pages/Productos/_CatalogoPdfChulin.cshtml`
- `Pages/Productos/_CatalogoPdfCardoza.cshtml`
- `Repositories/IProductoRepository.cs`
- `Repositories/ProductoRepository.cs`
- `Services/IProductoService.cs`
- `Services/ProductoService.cs`

## Decisiones de implementacion
- Se mantuvo una vista previa separada antes de descargar PDF.
- No se genera un PDF binario en servidor. La descarga se hace con `window.print()` y `Guardar como PDF`.
- Se usan dos plantillas distintas por sucursal porque el layout puede variar.
- El pie de pagina sigue usando imagenes desde `images_pdf_catalog/`.
- El encabezado de ambas sucursales se reconstruyo con HTML/CSS para poder mostrar la marca dinamicamente.
- La consulta del catalogo ordena por `Marca`, luego por `Descripcion`, y despues por `ProductoId`.
- La paginacion del catalogo se arma por grupos de marca y usa `9` productos por pagina en ambas sucursales.
- Cada hoja imprime su propia numeracion `Pagina X de Y`.

## Detalles por sucursal
### Chulin
- Plantilla: `_CatalogoPdfChulin.cshtml`
- Estilo visual basado en bloque izquierdo claro y bloque verde derecho.
- La marca actual se muestra en el bloque verde derecho del encabezado.

### Cardoza
- Plantilla: `_CatalogoPdfCardoza.cshtml`
- Estilo visual basado en encabezado rojo tipo ficha.
- La marca actual se muestra en el bloque central del encabezado.

## Consulta de datos
`ProductoRepository.ObtenerProductosCatalogoAsync(string? termino, bool soloConExistencia)`:
- aplica `LIKE` sobre `Producto.Descripcion`
- si `soloConExistencia` es `true`, filtra `Stock > 0`
- proyecta a `ProductoDTO`
- ordena por marca para que el render por paginas agrupe correctamente

## Imagenes y assets
- Las imagenes de referencia y pies estan en `ferreteria_catalog/images_pdf_catalog/`.
- Las imagenes de producto se leen desde `/images/{codigo}.jpg`.
- Si una imagen no existe, la plantilla usa `/images/folder.png`.

## Notas de mantenimiento
- Si cambia el layout de una sucursal, tocar solo su parcial.
- Si cambia la logica de filtros o el orden de productos, tocar primero `ObtenerProductosCatalogoAsync`.
- Si se quiere generar PDF real desde servidor en el futuro, la vista previa actual puede servir como base de HTML.
- La vista de login tiene loader visual en `Pages/Login/Login.cshtml` para indicar que el inicio de sesion esta en proceso.
