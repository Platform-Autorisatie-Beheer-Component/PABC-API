<template>
  <details>
    <summary>
      {{ functionalRole.name }}
    </summary>

    <p v-if="!functionalRole.mappings.length">
      Geen koppelingen gevonden voor deze functionele rol.
    </p>

    <p v-else>Hieronder zie je de koppelingen die op deze functionele rol van toepassing zijn.</p>

    <p>
      <button type="button" class="button secondary" @click="openAddDialog">
        <icon-container icon="plus" /> Toevoegen
      </button>
    </p>

    <item-list
      v-if="functionalRole.mappings.length"
      :items="functionalRole.mappings"
      @delete="openRemoveDialog"
    >
      <template #item="{ item: mapping }">
        <span>
          Is <strong>{{ mapping.name }}</strong>
          <template v-if="mapping.isAllEntityTypes">
            voor <strong>alle entiteittypes</strong></template
          >
          <template v-else-if="mapping.domain">
            binnen domein <strong>{{ mapping.domain }}</strong></template
          >
        </span>
      </template>
    </item-list>
  </details>

  <form-modal submit-type="create" @submit="handleCreate" ref="add-dialog">
    <functional-role-mappings-form
      v-model:mapping="mapping"
      :application-roles="applicationRoles"
      :domains="domains"
    />
  </form-modal>

  <form-modal submit-type="delete" @submit="handleRemove" ref="confirm-dialog">
    <h2>Koppeling verwijderen uit {{ functionalRole.name }}</h2>

    <p>
      Weet je zeker dat je koppeling <em>'{{ selectedMapping?.name }}'</em> wilt verwijderen?
    </p>
  </form-modal>
</template>

<script setup lang="ts">
import { computed, ref, useTemplateRef, type DeepReadonly } from "vue";
import ItemList from "@/components/ItemList.vue";
import FormModal from "@/components/FormModal.vue";
import IconContainer from "@/components/IconContainer.vue";
import { useFunctionalRoleMappings } from "@/composables/use-functional-role-mappings";
import type {
  ApplicationRole,
  Domain,
  FunctionalRoleMappings,
  Mapping
} from "@/services/pabcService";
import FunctionalRoleMappingsForm from "./FunctionalRoleMappingsForm.vue";

const { functionalRole } = defineProps<{
  functionalRole: DeepReadonly<FunctionalRoleMappings>;
  applicationRoles: DeepReadonly<ApplicationRole[]>;
  domains: DeepReadonly<Domain[]>;
}>();

const emit = defineEmits<{ (e: "refresh"): void }>();

const formDialog = useTemplateRef("add-dialog");
const confirmDialog = useTemplateRef("confirm-dialog");

const { addMapping, removeMapping } = useFunctionalRoleMappings();

const createEmptyMapping = (): Mapping => ({
  functionalRoleId: functionalRole.id,
  applicationRoleId: "",
  domainId: null,
  isAllEntityTypes: false
});

const mapping = ref(createEmptyMapping());

const selectedMappingId = ref("");

const selectedMapping = computed(() =>
  functionalRole.mappings.find((fr) => fr.id === selectedMappingId.value)
);

const openAddDialog = () => {
  mapping.value = createEmptyMapping();
  formDialog.value?.open();
};

const openRemoveDialog = (id: string) => {
  selectedMappingId.value = id;
  confirmDialog.value?.open();
};

const handleCreate = async () => {
  await addMapping(mapping.value);
  emit("refresh");
};

const handleRemove = async () => {
  await removeMapping(functionalRole.id, selectedMappingId.value);
  emit("refresh");
};
</script>
