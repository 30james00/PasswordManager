import { useAccountStore } from '@/stores/accountStore';
import EditPassword from '@/views/EditPassword.vue';
import CreatePassword from '@/views/CreatePassword.vue';
import Login from '@/views/Login.vue';
import Register from '@/views/Register.vue';
import SavedPasswordList from '@/components/SavedPasswordList.vue';
import AccountDetails from '@/views/AccountDetails.vue';
import HomeView from '@/views/HomeView.vue'
import { createRouter, createWebHistory } from 'vue-router';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/register',
      name: 'register',
      component: Register,
    },
    {
      path: '/login',
      name: 'login',
      component: Login,
    },
    {
      path: '/account',
      name: 'account',
      component: AccountDetails,
    },
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/passwords/create',
      name: 'password-create',
      component: CreatePassword,
    },
    {
      path: '/passwords/edit/:passwordId',
      name: 'password-edit',
      component: EditPassword,
      props: true,
    },
    // {
    //   path: "/about",
    //   name: "about",
    //   // route level code-splitting
    //   // this generates a separate chunk (About.[hash].js) for this route
    //   // which is lazy-loaded when the route is visited.
    //   component: () => import("../views/AboutView.vue"),
    // },
  ],
});

router.beforeEach((to) => {
  const accountStore = useAccountStore();
  const toName = to.name?.toString() ?? ""

  if (!['register', 'login'].includes(toName) && !accountStore.isLoggedIn) return { name: 'login' };
  if (['register', 'login'].includes(toName) && accountStore.isLoggedIn) return { name: 'password-list' };
});

export default router;
