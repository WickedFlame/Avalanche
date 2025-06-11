
export class Site {
    constructor() {
        let sw = document.querySelector('#lightswitch');
        if (sw) {
            sw.addEventListener('click', e => this.lightswitch());
        }
    }

    async lightswitch() {
        let htmlTag = document.getElementsByTagName('html')[0];

        if (htmlTag.classList.contains('theme-light')) {
            htmlTag.classList.remove('theme-light');
            localStorage.setItem('mode', 'dark');
        } else {
            htmlTag.classList.add('theme-light');
            localStorage.setItem('mode', 'light');
        }
    }
}