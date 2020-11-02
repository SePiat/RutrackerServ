
//window.onload = () => {
//    // устанавливаем настройки
//    const options = {
//        // родитель целевого элемента - область просмотра
//        root: null,
//        // без отступов
//        rootMargin: '0px',
//        // процент пересечения - половина изображения
//        threshold: 0.5
//    }

//    // создаем наблюдатель
//    const observer = new IntersectionObserver((entries, observer) => {
//        // для каждой записи-целевого элемента
//        entries.forEach(entry => {
//            // если элемент является наблюдаемым
//            if (entry.isIntersecting) {
//                alert("IntesectionAlarm")
//            }
//        })
//    }, options)

//    // с помощью цикла следим за всеми img на странице
//    const arr = document.querySelectorAll('#testContainer')
//    arr.forEach(i => {
//        observer.observe(i)
//    })

//}






   
