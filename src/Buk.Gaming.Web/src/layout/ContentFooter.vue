<template>
    <footer class="footer">
        <div class="container-fluid">
            <ul class="nav">
                <li class="nav-item">
                    <a
                        id="toornament-link"
                        href="https://www.toornament.com"
                        target="_blank"
                        ><img
                            width="160"
                            alt="https://www.toornament.com"
                            src="/img/PoweredbyToor_White.png"
                    /></a>
                </li>
            </ul>
            <div class="copyright">
                © {{ year }}
                <a style="color: white;" href="https://ungdomsklubben.org"
                    >BRUNSTAD UNGDOMSKLUBB</a
                >
                | <a href="/privacy-policy">PRIVACY POLICY</a>
            </div>
        </div>
    </footer>
</template>
<script>
export default {
    data() {
        return {
            year: new Date().getFullYear(),
        };
    },
};
</script>
<style></style>
