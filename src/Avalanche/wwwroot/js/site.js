
export class Site {
    constructor() {
        let sw = document.querySelector('#lightswitch');
        if (sw) {
            sw.addEventListener('click', e => this.lightswitch());
        }

        let tabs = document.querySelectorAll('.tabs');
        if (tabs) {
            tabs.forEach(e => this.registerTabs(e));
        }

        // Get all "navbar-burger" elements
        const $navbarBurgers = Array.prototype.slice.call(document.querySelectorAll('.navbar-burger'), 0);

        // Add a click event on each of them
        if ($navbarBurgers) {
            $navbarBurgers.forEach(el => {
                el.addEventListener('click', () => {

                    // Get the target from the "data-target" attribute
                    const target = el.dataset.target;
                    const $target = document.getElementById(target);

                    // Toggle the "is-active" class on both the "navbar-burger" and the "navbar-menu"
                    el.classList.toggle('is-active');
                    $target.classList.toggle('is-active');

                });
            });
        }
    }

    async lightswitch() {
        let htmlTag = document.getElementsByTagName('html')[0];
        let imgSrc = '/avalanche-logo-light.png';

        if (htmlTag.classList.contains('theme-light')) {
            htmlTag.classList.remove('theme-light');
            localStorage.setItem('mode', 'dark');    
        } else {
            htmlTag.classList.remove('theme-dark');
            htmlTag.classList.add('theme-light');
            localStorage.setItem('mode', 'light');
            imgSrc = '/avalanche-logo-dark.png';
        }

        let img = document.getElementById('avalanche-logo');
        if (img) {
            img.src = imgSrc;
        }

        img = document.getElementById('avalanche-logo-main');
        if (img) {
            img.src = imgSrc;
        }
    }

    async registerTabs(tabs) {
        tabs.querySelectorAll('a').forEach(link => {
            link.addEventListener('click', e => {
                // deselect all tabs
                tabs.querySelectorAll('li').forEach(t => t.classList.remove('is-active'));
                tabs.parentElement.querySelector('.tabs-content').querySelectorAll('li').forEach(t => t.classList.remove('is-active'));

                // select new tab
                e.target.parentElement.classList.add('is-active');
                document.querySelector(`#${e.target.dataset.tabid}`).classList.add('is-active');
            });
        });
    }
}